﻿using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CodeReview_V2.DataAccess;
using CodeReview_V2.Model;


namespace CodeReview_V2.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
		CodeReview codeReview = new CodeReview();

		public ObservableCollection<CustomFileObject> IncidentDataGrid { get { return incidentDataGrid; } }
		private ObservableCollection<CustomFileObject> incidentDataGrid = new ObservableCollection<CustomFileObject>();

		public ObservableCollection<string> FontFamilyCollection { get { return fontFamilyCollection; } }
		private ObservableCollection<string> fontFamilyCollection = new ObservableCollection<string>();
		
        public ListCollectionView IncidentDataGridCollectionView { get; set; }

        #region Command objects
        public Command SetGroupByProperty { get; set; }
        public Command GetFileDifference { get; set; }
		public Command FetchIncident { get; set; }
        #endregion //Command objects

		public string FontName { get; set; }
		public uint FontSize { get ; set; }
		public uint IncidentAssociationCount { get ; set; }

		public MainWindowViewModel()
		{
            IncidentDataGridCollectionView = new ListCollectionView(IncidentDataGrid);
            SetGroupByProperty = new Command(x => SetGroupPropertyDescription(x.ToString()));
			FetchIncident = new Command(x => GetIncident(x.ToString()));
			IncidentAssociationCount = 0;
			FontName = "Courier New";
			GetSupportedFonts();
		}

		private void GetSupportedFonts()
		{
			foreach (FontFamily f in FontFamily.Families)
				fontFamilyCollection.Add(f.Name);
		}

        private void SetGroupPropertyDescription(string groupPropertyDescription)
        {
            if (IncidentDataGridCollectionView.GroupDescriptions.Count > 0)
                IncidentDataGridCollectionView.GroupDescriptions.Clear();
            if (groupPropertyDescription != "Reset")
                IncidentDataGridCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(groupPropertyDescription));
        }

		public void GetIncident(string x)
		{
			uint incidentNo;
			if (!UInt32.TryParse(x, out incidentNo))
				return;

			Incident incident = codeReview.GetIncident(incidentNo);
			if (incident == null)
				return;

			PopulateIncidentDataGrid(incident);
			SetIncidentAssociationCount();
		}

		private void SetIncidentAssociationCount()
		{
			IncidentAssociationCount = (uint)IncidentDataGrid.Count;
			OnPropertyChanged("IncidentAssociationCount");
		}

		private void PopulateIncidentDataGrid(Incident incident)
		{
			IncidentDataGrid.Clear();
			foreach(CustomChangeset changeset in incident.ChangeSets)
			{
				foreach(FileItem file in changeset.Files)
				{
					//Here we can add filters for file that needs to be displayed and that can be ignored.
					IncidentDataGrid.Add(
						new CustomFileObject(file.Filename, changeset.CheckinChangeSet, changeset.CheckoutChangeSet, 
                                                 changeset.Comments, changeset.DevBranch, changeset.Author, Path.GetExtension(file.Filename))
						);
				}
			}
		}
	}

	public class CustomFileObject
	{
		public string Filename { get; set; }
		public string CheckoutChangeset { get; set; }
		public string CheckinChangeset { get; set; }
		public string Comments { get; set; }
		public string DevBranch { get; set; }
		public string Author { get; set; }
        public string FileType { get; set; }

		public CustomFileObject(string filename, string checkinChangeset, string checkoutchangeset, string comments, string devBranch, string author, string fileType)
		{
			Filename = filename;
			CheckinChangeset = checkinChangeset;
			CheckoutChangeset = checkoutchangeset;
			Comments = comments;
			DevBranch = devBranch;
			Author = author;
            FileType = fileType;
		}
	}
}
