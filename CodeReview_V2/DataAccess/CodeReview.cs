using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
	
using System.Threading.Tasks;

using CodeReview_V2.Model;

namespace CodeReview_V2.DataAccess
{
	public class CodeReview
	{
		TeamTrackAccess tt;
		TFSAccess tf;

		public CodeReview()
		{
			tt = new TeamTrackAccess();
			tf = new TFSAccess();
		}

		public Incident GetIncident(uint incidentNo)
		{
			Incident incident = new Incident();

			incident = tt.GetIncident(incidentNo);

			//The checkin's from the incident branch are not recorded in the teamtrack incident.
			//The teamtrack incident only has the branch and merge information.
			//TFS access is required to get the changesets in the incident branch. These changesets will then
			//be converted to ITeamtrack.Association
			//Get the associations "Incident/#####"
			List<CustomChangeset> incidentBranches = incident.ChangeSets.Where(x => x.IncidentBranch == true).ToList<CustomChangeset>();
			//Get each changeset in the Incident branch
			incident.ChangeSets.AddRange(tf.GetIncidentChanges(incidentBranches));
			return incident;
		}

		public void GetFileDifference(string filename, string checkoutChangeset, string checkinChangeset)
		{
			string argument = String.Format("difference {0} /version:C{1}~C{2} /format:visual", filename, checkoutChangeset, checkinChangeset);
			tf.RunTF<int>(argument, false, -1);
		}
	}
}
