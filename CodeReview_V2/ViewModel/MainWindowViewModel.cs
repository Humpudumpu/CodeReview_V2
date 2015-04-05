using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using CodeReview_V2.DataAccess;
using CodeReview_V2.Model;


namespace CodeReview_V2.ViewModel
{
	public class MainWindowViewModel : ViewModelBase
	{
        const string location = "HOME";
		//CodeReview codeReview = new CodeReview();
		public ObservableCollection<CustomFileObject> IncidentDataGrid { get { return incidentDataGrid; } }
		private ObservableCollection<CustomFileObject> incidentDataGrid = new ObservableCollection<CustomFileObject>();

        public ListCollectionView IncidentDataGridCollectionView { get; set; }

        #region Command objects
        public Command SetGroupByProperty { get; set; }
        public Command GetFileDifference { get; set; }
        #endregion //Command objects

		public MainWindowViewModel()
		{
			GetIncident(72382);
            IncidentDataGridCollectionView = new ListCollectionView(IncidentDataGrid);
            SetGroupByProperty = new Command(x => SetGroupPropertyDescription(x.ToString()));
		}

        private void SetGroupPropertyDescription(string groupPropertyDescription)
        {
            if (IncidentDataGridCollectionView.GroupDescriptions.Count > 0)
                IncidentDataGridCollectionView.GroupDescriptions.Clear();
            IncidentDataGridCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(groupPropertyDescription));
        }

		public void GetIncident(uint incidentNo)
		{
            Incident incident = null;
            if (location == "WORK")
            {
                //incident = codeReview.GetIncident(incidentNo);
                if (incident == null)
                    return;
            }
            else
                incident = new Incident();

			PopulateIncidentDataGrid(incident);
		}

		private void PopulateIncidentDataGrid(Incident incident)
		{
			IncidentDataGrid.Clear();
            if (location == "WORK")
            {
                foreach (CustomChangeset changeset in incident.ChangeSets)
                {
                    foreach (FileItem file in changeset.Files)
                    {
                        //Here we can add filters for file that needs to be displayed and that can be ignored.
                        IncidentDataGrid.Add(
                            new CustomFileObject(file.Filename, changeset.CheckinChangeSet, changeset.CheckoutChangeSet,
                                                 changeset.Comments, changeset.DevBranch, changeset.Author)
                            );
                    }
                }
            }
            else
                PopulateIncidentDataGrid();
		}

        private void PopulateIncidentDataGrid()
        {
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2dev\file1", "22", "", "Comment1", "", "TestAuthor1"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2dev\file2", "23", "", "Comment2", "", "TestAuthor2"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2dev\file3", "24", "", "Comment3", "", "TestAuthor3"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2devfile4", "25", "", "Comment4", "", "TestAuthor4"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2devfile5", "26", "", "Comment5", "", "TestAuthor5"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2devfile6", "27", "", "Comment6", "", "TestAuthor6"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2devfile7", "28", "", "Comment7", "", "TestAuthor7"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2devfile8", "29", "", "Comment8", "", "TestAuthor8"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2devfile9", "30", "", "Comment9", "", "TestAuthor9"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2devfile10", "31", "", "Comment10", "", "TestAuthor10"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2devfile11", "32", "", "Comment11", "", "TestAuthor11"));
            IncidentDataGrid.Add(new CustomFileObject(@"C:\USCAN\Product\5.2\5.2devfile12", "33", "", "Comment12", "", "TestAuthor12"));
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
