using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.Generic;
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
		public Command CopyChangesetText { get; set; }
		public Command ToggleCodeReviewNoteVisibility { get; set; }
        #endregion //Command objects

		public string FontName { get; set; }
		public uint FontSize { get ; set; }
		public uint IncidentAssociationCount { get ; set; }
		public string CodeReviewText { get; set; }
		public Visibility CodeReviewNotesVisibility { get; set; }

		public MainWindowViewModel()
		{
            IncidentDataGridCollectionView = new ListCollectionView(IncidentDataGrid);
			GetFileDifference = new Command(x => FileDifference(x));
            SetGroupByProperty = new Command(x => SetGroupPropertyDescription(x.ToString()));
			FetchIncident = new Command(x => GetIncident(x.ToString()));
			CopyChangesetText = new Command(x => AddToCodeReviewNote(x));
			ToggleCodeReviewNoteVisibility = new Command(x => ToggleCodeReviewNotes(x));
			IncidentAssociationCount = 0;
			FontName = "Courier New";
			CodeReviewText = String.Empty;
			CodeReviewNotesVisibility = Visibility.Collapsed;
			GetSupportedFonts();
		}

		private void FileDifference(object FileObjects)
		{
			List<CustomFileObject> associations = ((IEnumerable)FileObjects).Cast<CustomFileObject>().ToList();
			string diffFilename = String.Empty;
			string checkout = string.Empty;
			string checkin = string.Empty;
			if (associations.Count > 1)
			{
				CustomFileObject minCheckedOutFile  = null;
				CustomFileObject maxCheckedInFile = null;
				try
				{
					minCheckedOutFile = associations.Aggregate((c, d) => Convert.ToInt32(c.CheckoutChangeset) < Convert.ToInt32(d.CheckoutChangeset) ? c : d);
					maxCheckedInFile = associations.Aggregate((c, d) => Convert.ToInt32(c.CheckinChangeset) > Convert.ToInt32(d.CheckinChangeset) ? c : d);
				}
				catch (Exception) 
				{
					//Some file does not have the checkout or checkin changeset number.
					return;
				}

				if (minCheckedOutFile.Filename != maxCheckedInFile.Filename)
				{
					//status message;
					Console.WriteLine("Choose same file");
					return;
				}
				diffFilename = minCheckedOutFile.Filename;
				checkout = minCheckedOutFile.CheckoutChangeset;
				checkin = maxCheckedInFile.CheckinChangeset;
			}
			else
			{
				diffFilename = associations[0].Filename;
				checkout = associations[0].CheckoutChangeset;
				checkin = associations[0].CheckinChangeset;
			}
			if (checkout.Equals(String.Empty) || checkin.Equals(String.Empty))
				return;
			codeReview.GetFileDifference(diffFilename, checkout, checkin);
		}

		private void ToggleCodeReviewNotes(object fileObject)
		{
			CodeReviewNotesVisibility = CodeReviewNotesVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
			OnPropertyChanged("CodeReviewNotesVisibility");
		}

		private void SaveCodeReviewText()
		{
			//Not implemented
			return;
		}

		private void AddToCodeReviewNote(object customFileObject)
		{
			if (IncidentAssociationCount == 0)
				return;
			CustomFileObject fileObject = customFileObject as CustomFileObject;
			CodeReviewText += "\n" + fileObject.ToString();
			OnPropertyChanged("CodeReviewText");
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
						new CustomFileObject(file.Filename, file.CheckinChangeset, file.CheckoutChangeset,
                                                 changeset.Comments, changeset.DevBranch, changeset.Author, Path.GetExtension(file.Filename), changeset.CheckinTime, 
												 changeset.IsChangesetMergedToDev, changeset.IsDevChangesetMergedToInt)
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
		public DateTime CheckinTime { get; set; }
		public bool DevChangesetMerged { get; set; }
		public bool IntChangesetMerged { get; set; }

		public CustomFileObject(string filename, string checkinChangeset, string checkoutchangeset, string comments, string devBranch, string author, string fileType, DateTime dateTime,
								bool devChangesetMerged, bool intChangesetMerged)
		{
			Filename = filename;
			CheckinChangeset = checkinChangeset;
			CheckoutChangeset = checkoutchangeset;
			Comments = comments;
			DevBranch = devBranch;
			Author = author;
            FileType = fileType;
			CheckinTime = dateTime;
			DevChangesetMerged = devChangesetMerged;
			IntChangesetMerged = intChangesetMerged;
		}

		public override string ToString()
		{
			string customFileObjectAsText = String.Empty;
			customFileObjectAsText = String.Format("Filename: {0} ; Checkin : {1} ; Checkout : {2}", this.Filename, this.CheckinChangeset, this.CheckoutChangeset);
			return customFileObjectAsText;
		}
	}
}
