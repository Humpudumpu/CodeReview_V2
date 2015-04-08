using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
	
using System.Threading.Tasks;

using CodeReview_V2.Model;

namespace CodeReview_V2.DataAccess
{
	public class CodeReview
	{
		TeamTrackAccess ttAccess;
		TFSAccess tfsAccess;

		public CodeReview()
		{
			ttAccess = new TeamTrackAccess();
			tfsAccess = new TFSAccess();
		}

		public Incident GetIncident(uint incidentNo)
		{
			Incident incident = new Incident();

			incident = ttAccess.GetIncident(incidentNo);

			//The checkin's from the incident branch are not recorded in the teamtrack incident.
			//The teamtrack incident only has the branch and merge information.
			//TFS access is required to get the changesets in the incident branch. These changesets will then
			//be converted to ITeamtrack.Association
			//Get the associations "Incident/#####"
			List<CustomChangeset> incidentBranches = incident.ChangeSets.Where(x => x.IncidentBranch == true).ToList<CustomChangeset>();
			//Get each changeset in the Incident branch
			incident.ChangeSets.AddRange(tfsAccess.GetIncidentChanges(incidentBranches));
			incident = AssignCheckoutChangeset(incident);
			return incident;
		}
		//Complicated code. If possible at some later stage we can simplify the code.
		private Incident AssignCheckoutChangeset(Incident incident)
		{
			Dictionary<string,List<string>> fileChangesets = new Dictionary<string,List<string>>();
			string branchedChangeSet = String.Empty;
			try
			{
				foreach (CustomChangeset changeset in incident.ChangeSets)
				{
					if (changeset.IncidentBranch)
						branchedChangeSet = changeset.CheckinChangeSet;

					foreach (FileItem file in changeset.Files)
					{
						if (!fileChangesets.ContainsKey(file.Filename))
						{
							List<string> changesets = new List<string>();
							changesets.Add(file.CheckinChangeset);
							fileChangesets.Add(file.Filename, changesets);
						}
						else
						{
							List<string> changesets;
							fileChangesets.TryGetValue(file.Filename, out changesets);
							changesets.Add(file.CheckinChangeset);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception in AssignCheckoutChangeset : " + ex.Message);
			}

			for (int j = 0; j < incident.ChangeSets.Count; j++)
			{
				for (int i = 0; i < incident.ChangeSets[j].Files.Count; i++)
				{
					List<string> fileInvolvedChangesets;
					fileChangesets.TryGetValue(incident.ChangeSets[j].Files[i].Filename, out fileInvolvedChangesets);
					//sort in ascending order
					fileInvolvedChangesets.Sort();
					//Find the index of checkin changeset in the changeset list
					int index = fileInvolvedChangesets.FindIndex(x => x.Equals(incident.ChangeSets[j].Files[i].CheckinChangeset));

					//The checkout changeset will be the one before the checkin changeset always
					if (index - 1 >= 0)
						incident.ChangeSets[j].Files[i].CheckoutChangeset = fileInvolvedChangesets[index - 1];
				}
			}
			return incident;
		}

		public void GetFileDifference(string filename, string checkoutChangeset, string checkinChangeset)
		{
			string argument = String.Format("difference {0} /version:C{1}~C{2} /format:visual", filename, checkoutChangeset, checkinChangeset);
			tfsAccess.RunTF<int>(argument, false, -1);
		}
	}
}
