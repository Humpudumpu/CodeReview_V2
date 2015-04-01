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
				Changeset removed = incidentBranchHistory.Last();
				incidentBranchHistory.Remove(removed);
				changesets.AddRange(ParseTFSOutput(incidentBranchHistory));
				CustomChangeset removedCustom = new CustomChangeset("", removed.Comment, "", removed.ChangesetId.ToString(), removed.CommitterDisplayName);
				//This is to avoid getting all the files from the branched changeset when the GetChangeSet command is used.
				changesets.Add(removedCustom);
			}
			return changesets;
		}

		private List<CustomChangeset> ParseTFSOutput(List<Changeset> changesets)
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
				incidentBranchChangeSet.Add(change);

				//Run tfs to get the files in the changeset.
				var tfsChangeset = service.GetChangeset(changesetEntry.ChangesetId);
				Change[] tfschanges = tfsChangeset.Changes;
				foreach (Change tfschange in tfschanges)
					change.Files.Add(FileItem.CreateFileItem(tfschange.Item.ServerItem));
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
					standardOutput = tf.StandardOutput.ReadLine();

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
