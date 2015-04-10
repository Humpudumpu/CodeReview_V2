﻿using System;
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

		public List<CustomChangeset> GetIncidentChanges(List<CustomChangeset> incidentBranches)
		{
			//Here the incident branch just has the branch name. 
			//Here the list of changeset will have each file that was changed in the incidentbranch.
			//This information is not available in Teamtrack, so it has to be extracted from tfs.
			List<CustomChangeset> changesets = new List<CustomChangeset>();
			TfsTeamProjectCollection tfs = new TfsTeamProjectCollection(new Uri(@"http://can10tfsprd1:8080/tfs/can10tpc4"));
			var service = tfs.GetService<VersionControlServer>();

			foreach (CustomChangeset incidentBranchPath in incidentBranches)
			{
				//assumption is that the incidentbranch will be just one. so there will be just one file association.
				List<Changeset> incidentBranchHistory = service.QueryHistory(incidentBranchPath.Files.First().Filename, RecursionType.Full).ToList<Changeset>();
				//$/USCAN/Product/5.0SON/Incidents/72382
				//Match 5.0SON/Incidents/72382 -> Groups[1] will return $/USCAN/Product/5.0SON
				Regex productPath = new Regex(@"(.*)/Incidents/\d+$", RegexOptions.IgnoreCase);
				string productPathValue = productPath.Match(incidentBranchPath.Files.First().Filename).Groups[1].Value;

				//Get all the branches under the product : so all branches under $/USCAN/Product/5.0SON
				List<string> branchesUnderProductBranch = RunTF<string>(String.Format("dir /collection:{0} /version:T /folders {1}", @"http://can10tfsprd1:8080/tfs/can10tpc4" , productPathValue))
										.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
				//Isolate the dev branch
				Regex devBranch = new Regex(@"^\$(.*)dev$");
				string devBranchPath = productPathValue;
				string devBranchTest = String.Empty;
				foreach (string s in branchesUnderProductBranch)
					if (devBranch.IsMatch(s))
					{
						//result here: $/USCAN/Product/5.0SON/5.0dev
						devBranchTest = s.Replace("$", "");
						devBranchTest = devBranchTest.Replace("dev", "");
						devBranchPath += s.Replace("$", "/");
						break;
					}

				//Get the merges from source : $/USCAN/Product/5.0SON/Incidents/##### to $/USCAN/Product/5.0SON/##dev
				//This is equivalent to : tf.exe merges [source] destination /recursive /version:T
				List<ChangesetMerge> merges = 
					service.QueryMerges(new ItemSpec(incidentBranchPath.Files.First().Filename, RecursionType.Full), VersionSpec.Latest, 
									    new ItemSpec(devBranchPath, RecursionType.Full), VersionSpec.Latest, null, null).ToList<ChangesetMerge>();

				//Remove the branch changeset number - the changeset where the incident branch started.
				Changeset removed = incidentBranchHistory.Last();
				incidentBranchHistory.Remove(removed);
				changesets.AddRange(ParseTFSOutput(incidentBranchHistory, merges, devBranchTest));

				//This is to avoid the last changeset - The changeset that was branched. (For ex. in Incident#72382 - changeset #222
				//Why avoid last changeset because = file changes in the branch changeset is all the files in the dev branch. So when I query change for the 
				//branch changeset, it gives me all the files in the branch - But in reality the change is that it was branched from the dev branch. 
				//So I create a dummy custom changeset. Enumerate all the files that were changed in the incident branch and assign the changesetID of the branch - i.e. according to the ex. changeset#222
				CustomChangeset removedCustom = new CustomChangeset("", removed.Comment, "", removed.ChangesetId.ToString(), removed.CommitterDisplayName);
				
				//Enumerate all files that were changed in the incident branch after branching.
				HashSet<string> allFilesModifiedInIncidentBranch = new HashSet<string>();
				//Go through all the changeset other than the branch changeset, get all the files and assign the checkinchangeset as the branch changeset number.
				//This is basically creating a changeset with only the files that were changes in the incident branch. (reverse engineering)
				foreach (CustomChangeset changsetC in changesets)
					foreach (FileItem file in changsetC.Files)
						allFilesModifiedInIncidentBranch.Add(file.Filename);
				
				//Now assign all the files that were changed in the incident branch with the branch changeset number.
				foreach (string file in allFilesModifiedInIncidentBranch)
					removedCustom.Files.Add(FileItem.CreateFileItem(file, removed.ChangesetId.ToString(), ChangeType.Branch));

				changesets.Add(removedCustom);
			}
			return changesets;
		}

		private List<CustomChangeset> ParseTFSOutput(List<Changeset> changesets, List<ChangesetMerge> merges, string devBranch)
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
				ChangesetMerge x = merges.Where(y => y.SourceVersion == changesetEntry.ChangesetId).First();
				change.MergedToChangeset = (x != null) ? x.TargetVersion.ToString() : String.Empty;
				change.ChangesetMerged = change.MergedToChangeset != String.Empty ? true : false;
				change.DevBranch = devBranch;
				incidentBranchChangeSet.Add(change);

				//Run tfs to get the files in the changeset.
				var tfsChangeset = service.GetChangeset(changesetEntry.ChangesetId);
				Change[] tfschanges = tfsChangeset.Changes;
				foreach (Change tfschange in tfschanges)
				{
					change.Files.Add(FileItem.CreateFileItem(tfschange.Item.ServerItem, changesetEntry.ChangesetId.ToString(), tfschange.ChangeType));
				}
			}
			return incidentBranchChangeSet;
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
