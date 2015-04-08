using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace CodeReview_V2.Model
{
	public class FileItem
	{
		public string Filename { get { return filename; } set { filename = value; } }
		private string filename;

		public string CheckinChangeset { get { return checkinChangeset; } set { checkinChangeset = value; } }
		private string checkinChangeset;

		public string CheckoutChangeset { get { return checkoutChangeset; } set { checkoutChangeset = value; } }
		private string checkoutChangeset;

		public ChangeType FileChangeType { get; set; }

		protected FileItem() { }

		public static FileItem CreateFileItem(string filename, string checkin, ChangeType changeType = ChangeType.None)
		{
			return new FileItem {
			Filename = filename,
			FileChangeType = changeType,
			CheckinChangeset = checkin,
			CheckoutChangeset = String.Empty
			};
		}
	}
}
