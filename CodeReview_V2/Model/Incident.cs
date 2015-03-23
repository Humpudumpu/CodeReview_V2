using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeReview_V2.Properties;

namespace CodeReview_V2.Model
{
	public class Incident
	{
		public List<Changeset> ChangeSets { get { return changesets; } set { changesets = value; } }
		private List<Changeset> changesets;

		public string IncidentTitle { get; set; }
		public string IncidentURL { get; set; }

		public Incident()
		{
			ChangeSets = new List<Changeset>();
			IncidentTitle = Strings.Application_WindowTitle;
			IncidentURL = @"http://can10-teamtrack/tmtrack/tmtrack.dll?";
		}
	}
}
