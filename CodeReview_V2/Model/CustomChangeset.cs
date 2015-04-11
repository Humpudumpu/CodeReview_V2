using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReview_V2.Model
{
	public class CustomChangeset
	{
		public List<FileItem> Files { get { return files; } set { files = value; } }
		private List<FileItem> files;

		public string DevBranch { get { return devBranch; } set { devBranch = value; } }
		private string devBranch;

		public string Comments { get { return comments; } set { comments = value; } }
		private string comments;

		public string CheckoutChangeSet { get { return checkoutChangeSet; } set { checkoutChangeSet = value; } }
		private string checkoutChangeSet;

		public string CheckinChangeSet { get { return checkinChangeSet; } set { checkinChangeSet = value; } }
		private string checkinChangeSet;

		public string Author { get { return author; } set { author = value; } }
		private string author;

		public bool IncidentBranch { get { return incidentBranch; } set { incidentBranch = value; } }
		private bool incidentBranch;

		public DateTime CheckinTime { get { return checkinTime; } set { checkinTime = value; } }
		private DateTime checkinTime;

		public bool IsChangesetMergedToDev { get; set; }
		public string DevChangesetMergedTo { get; set; }

		public bool IsDevChangesetMergedToInt { get; set; }
		public string IntChangesetMergedTo { get; set; }

		private void Init(){
			Files = new List<FileItem>();
			DevBranch = String.Empty;
			Comments = String.Empty;
			CheckoutChangeSet = String.Empty;
			CheckinChangeSet = String.Empty;
			Author = String.Empty;
			IncidentBranch = false;
			CheckinTime = System.DateTime.Now;
			IsChangesetMergedToDev = false;
			DevChangesetMergedTo = String.Empty;
			IsDevChangesetMergedToInt = false;
			IntChangesetMergedTo = String.Empty;
		}

		public CustomChangeset()
		{
			Init();
		}


		public CustomChangeset(string devBranch, string comments, string checkout, string checkin, string author)
		{
			Init();
			DevBranch = devBranch;
			Comments = comments;
			CheckoutChangeSet = checkout;
			CheckinChangeSet = checkin;
			Author = author;
			IsChangesetMergedToDev = false;
			DevChangesetMergedTo = String.Empty;
			IsDevChangesetMergedToInt = false;
			IntChangesetMergedTo = String.Empty;
		}
	}
}
