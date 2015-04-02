using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeReview_V2.DataAccess;
using CodeReview_V2.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace CodeReview_V2.ViewModel
{
	public class MainWindowViewModel
	{
		CodeReview codeReview = new CodeReview();
		public ObservableCollection<CustomFileObject> IncidentDataGrid { get { return incidentDataGrid; } }
		private ObservableCollection<CustomFileObject> incidentDataGrid = new ObservableCollection<CustomFileObject>();

		public MainWindowViewModel()
		{
			GetIncident(72382);
		}

		public void GetIncident(uint incidentNo)
		{
			Incident incident = codeReview.GetIncident(incidentNo);
			if (incident == null)
				return;

			PopulateIncidentDataGrid(incident);
		}

		private void PopulateIncidentDataGrid(Incident incident)
		{
			IncidentDataGrid.Clear();
			List<CustomFileObject> fileObjects = new List<CustomFileObject>();
			foreach(CustomChangeset changeset in incident.ChangeSets)
			{
				foreach(FileItem file in changeset.Files)
				{
					//Here we can add filters for file that needs to be displayed and that can be ignored.
					IncidentDataGrid.Add(
						new CustomFileObject(file.Filename, changeset.CheckinChangeSet, changeset.CheckoutChangeSet, 
											 changeset.Comments, changeset.DevBranch, changeset.Author)
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

		public CustomFileObject(string filename, string checkinChangeset, string checkoutchangeset, string comments, string devBranch, string author)
		{
			Filename = filename;
			CheckinChangeset = checkinChangeset;
			CheckoutChangeset = checkoutchangeset;
			Comments = comments;
			DevBranch = devBranch;
			Author = author;
		}
	}
}
