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

		public string IncidentTitle { get; set; }
		public string IncidentURL { get; set; }

		public Incident()
		{
			ChangeSets = new List<CustomChangeset>();
			IncidentTitle = Strings.Application_WindowTitle;
			IncidentURL = @"http://can10-teamtrack/tmtrack/tmtrack.dll?";
		}

		public void clear()
		{
			IncidentTitle = String.Empty;
			IncidentURL = @"http://can10-teamtrack/tmtrack/tmtrack.dll?";
			ChangeSets.Clear();
		}
	}
}
