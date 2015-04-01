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
		public ObservableCollection<Incident> IncidentDataGrid { get { return incidentDataGrid; } }
		private ObservableCollection<Incident> incidentDataGrid = new ObservableCollection<Incident>();

		public MainWindowViewModel()
		{

		}

		public void GetIncident(uint incidentNo)
		{
			codeReview.GetIncident(incidentNo);
		}
	}
}
