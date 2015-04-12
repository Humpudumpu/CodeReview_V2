using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;

using CodeReview_V2.Model;

namespace CodeReview_V2.DataAccess
{
	public class TFSAccess
	{
		public string Tfs { get { return tfs; } set { tfs = value; } }
		private string tfs;

		public TFSAccess()
		{
			Tfs = Path.Combine(Environment.GetEnvironmentVariable("VS110COMNTOOLS").Replace("Tools", "IDE"), "tf.exe");
		}

		public List<CustomChangeset> GetIncidentChanges(string incidentBranch, string devBranchPath)
		{
			//Here the incident branch just has the branch name. 
			//Here the list of changeset will have each file that was changed in the incidentbranch.
			//This information is not available in Teamtrack, so it has to be extracted from tfs.
			List<CustomChangeset> changesets = new List<CustomChangeset>();
			TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(@"http://can10tfsprd1:8080/tfs/can10tpc4"));
			var service = tfs.GetService<VersionControlServer>();

            //#Future: If there are more than one incident branch in an incident. Put a loop like : foreach(CustomChangeset incidentBranchPath in incidentBranches){}
            //Now: since we assume that there is just one incident branch associated to each incident -


			//assumption is that the incidentbranch will be just one. so there will be just one file association.
			List<Changeset> incidentBranchHistory = service.QueryHistory(incidentBranch, RecursionType.Full).Distinct().ToList<Changeset>();

			//Get the merges from source : $/USCAN/Product/5.0SON/Incidents/##### to $/USCAN/Product/5.0SON/##dev
			//This is equivalent to : tf.exe merges [source] destination /recursive /version:T
			List<ChangesetMerge> incidentToDevBranchMerges = 
				service.QueryMerges(new ItemSpec(incidentBranch, RecursionType.Full), VersionSpec.Latest, 
									new ItemSpec(devBranchPath, RecursionType.Full), VersionSpec.Latest, null, null).Distinct().ToList<ChangesetMerge>();

			//Remove the branch changeset number - the changeset where the incident branch started.
			Changeset removed = incidentBranchHistory.Last();
			incidentBranchHistory.Remove(removed);
			changesets.AddRange(PopulateWithChangesetsFromIncidentBranch(incidentBranchHistory, incidentToDevBranchMerges));
	
			//This is to avoid the last changeset - The changeset that was branched. (For ex. in Incident#72382 - changeset #222
			//Why avoid last changeset because = file changes in the branch changeset is all the files in the dev branch. So when I query change for the 
			//branch changeset, it gives me all the files in the branch - But in reality the change is that it was branched from the dev branch. 
			//So I create a dummy custom changeset. Enumerate all the files that were changed in the incident branch and assign the changesetID of the branch - i.e. according to the ex. changeset#222
			CustomChangeset removedCustom = new CustomChangeset(String.Empty, removed.Comment, String.Empty, removed.ChangesetId.ToString(), removed.CommitterDisplayName);
				
			//Go through all the changeset other than the branch changeset, get all the files and assign the checkinchangeset as the branch changeset number.
			//This is basically creating a changeset with only the files that were changes in the incident branch. (reverse engineering)
			List<string> files = changesets.SelectMany(h => h.Files)
				.Select(filenames => filenames.Filename).Distinct().ToList();
				
			//Now assign all the files that were changed in the incident branch with the branch changeset number.
			foreach (string file in files)
				removedCustom.Files.Add(FileItem.CreateFileItem(file, removed.ChangesetId.ToString(), ChangeType.Branch));

			changesets.Add(removedCustom);
			
			return changesets;
		}

		private List<CustomChangeset> PopulateWithChangesetsFromIncidentBranch(List<Changeset> changesets, List<ChangesetMerge> devMerges)
		{
			TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(@"http://can10tfsprd1:8080/tfs/can10tpc4"));
			var service = tfs.GetService<VersionControlServer>();

			List<CustomChangeset> incidentBranchChangeSet = new List<CustomChangeset>();
			foreach (Changeset changesetEntry in changesets)
			{
				CustomChangeset change = new CustomChangeset();
				change.CheckinChangeSet = changesetEntry.ChangesetId.ToString();
				change.Author = changesetEntry.CommitterDisplayName;
				change.CheckinTime = changesetEntry.CreationDate;
				change.Comments = changesetEntry.Comment;

				//Determine if incident changeset was added to dev branch
				ChangesetMerge x = devMerges.Where(y => y.SourceVersion == changesetEntry.ChangesetId).First();
				change.DevChangesetMergedTo = (x != null) ? x.TargetVersion.ToString() : String.Empty;
				change.IsChangesetMergedToDev = change.DevChangesetMergedTo != String.Empty ? true : false;

				incidentBranchChangeSet.Add(change);

				//Run tfs to get the files in the changeset.
				var tfsChangeset = service.GetChangeset(changesetEntry.ChangesetId);
				Change[] tfschanges = tfsChangeset.Changes;
				foreach (Change tfschange in tfschanges)
					change.Files.Add(FileItem.CreateFileItem(tfschange.Item.ServerItem, changesetEntry.ChangesetId.ToString(), tfschange.ChangeType));
			}
			return incidentBranchChangeSet;
		}

		//Called from CodeReview class
		public List<CustomChangeset> ValidateDevBranchIsMergedToIntegrationBranch(List<CustomChangeset> incidentChangesets, string devBranchPath, string integrationBranchPath)
		{
			TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(@"http://can10tfsprd1:8080/tfs/can10tpc4"));
			var service = tfs.GetService<VersionControlServer>();

			//Get the merges from source : $/USCAN/Product/5.0SON/##dev to $/USCAN/Product/5.0SON/Builds/##int
			//This is equivalent to : tf.exe merges [source] destination /recursive /version:T
			List<ChangesetMerge> devToIntegrationBranchMerges =
				service.QueryMerges(new ItemSpec(devBranchPath, RecursionType.Full), VersionSpec.Latest,
									new ItemSpec(integrationBranchPath, RecursionType.Full), VersionSpec.Latest, null, null).Distinct().ToList<ChangesetMerge>();

			foreach(CustomChangeset change in incidentChangesets)
			{
				foreach(FileItem file in change.Files)
				{
					if (file.Filename.Contains(devBranchPath))
					{
						ChangesetMerge y = devToIntegrationBranchMerges.Where(z => z.SourceVersion.ToString() == change.CheckinChangeSet).First();
						change.IntChangesetMergedTo = (y != null) ? y.TargetVersion.ToString() : String.Empty;
						change.IsDevChangesetMergedToInt = change.IntChangesetMergedTo != String.Empty ? true : false;
					}
				}
			}
			return incidentChangesets;
		}

		public T RunTF<T>(string arguments, bool redirectOutput = true, int waitMinutes = 2)
		{
			Directory.SetCurrentDirectory(@"C:\Work2");
			string standardOutput = String.Empty;
			Process tf = new Process();
			tf.StartInfo.FileName = Tfs;
			tf.StartInfo.Arguments = arguments;
			tf.StartInfo.RedirectStandardOutput = redirectOutput;
			tf.StartInfo.CreateNoWindow = true;
			tf.StartInfo.UseShellExecute = false;
			tf.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

			try
			{
				tf.Start();
				if (tf.StartInfo.RedirectStandardOutput)
					standardOutput = tf.StandardOutput.ReadToEnd();

				if (waitMinutes > 0)
				{
					tf.WaitForExit(waitMinutes * 60 * 1000);
					return tf.StartInfo.RedirectStandardOutput ? (T)(object)standardOutput : (T)(object)tf.ExitCode;
				}
				else
					return (T)(object)0;
			}
			finally { tf.Close(); }
		}

		#region helper
		
		#endregion
	}
}
