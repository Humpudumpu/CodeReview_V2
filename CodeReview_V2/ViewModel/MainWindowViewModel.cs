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

		}

		public void GetIncident(uint incidentNo)
		{
			codeReview.GetIncident(incidentNo);
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
