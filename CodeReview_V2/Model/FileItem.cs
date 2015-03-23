using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReview_V2.Model
{
	public class FileItem
	{
		public string Filename { get { return filename; } set { filename = value; } }
		private string filename;

		public string Comments { get { return comments; } set { comments = value; } }
		private string comments;

		public string CheckoutChangeSet { get { return checkoutChangeSet; } set { checkoutChangeSet = value; } }
		private string checkoutChangeSet;

		public string CheckinChangeSet { get { return checkinChangeSet; } set { checkinChangeSet = value; } }
		private string checkinChangeSet;

		public string Author { get { return author; } set { author = value; } }
		private string author;

		protected FileItem() { }

		public static FileItem CreateFileItem(string filename, string comments, string checkoutNo, string checkinNo, string author)
		{
			return new FileItem {
			Filename = filename,
			CheckoutChangeSet = checkoutNo,
			CheckinChangeSet = checkinNo,
			Author = author,
			Comments = comments
			};
		}
	}
}
