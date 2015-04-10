using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeReview_V2.Properties;

namespace CodeReview_V2.Model
{
	public class Incident : CustomChangeset
	{
		public List<CustomChangeset> ChangeSets { get { return changesets; } set { changesets = value; } }
		private List<CustomChangeset> changesets;

		public string IncidentProductPath { get; set; }
		public string IncidentBranchPath { get; set; }
		public string IncidentDevBranchPath { get; set; }
		public string IncidentIntegrationBranchPath { get; set; }

		public string IncidentProductName { get; set; }
		public string IncidentBranchName { get; set; }
		public string IncidentDevBranchName { get; set; }
		public string IncidentIntegrationBranchName { get; set; }

		public string IncidentTitle { get; set; }
		public string IncidentURL { get; set; }

		public Incident()
		{
			ChangeSets = new List<CustomChangeset>();
			IncidentTitle = Strings.Application_WindowTitle;
			IncidentURL = @"http://can10-teamtrack/tmtrack/tmtrack.dll?";
			
			IncidentProductName = String.Empty;
			IncidentProductPath = String.Empty;
			
			IncidentBranchPath = String.Empty;
			IncidentBranchName = String.Empty;
			
			IncidentDevBranchPath = String.Empty;
			IncidentDevBranchName = String.Empty;
			
			IncidentIntegrationBranchPath = String.Empty;
			IncidentIntegrationBranchName = String.Empty;
		}

		public void clear()
		{
			IncidentTitle = String.Empty;
			IncidentProductName = String.Empty;
			IncidentProductPath = String.Empty;
			IncidentBranchPath = String.Empty;
			IncidentBranchName = String.Empty;
			IncidentDevBranchPath = String.Empty;
			IncidentDevBranchName = String.Empty;
			IncidentIntegrationBranchPath = String.Empty;
			IncidentIntegrationBranchName = String.Empty;
			IncidentURL = @"http://can10-teamtrack/tmtrack/tmtrack.dll?";
			ChangeSets.Clear();
		}
	}
}
