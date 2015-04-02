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

		public string CheckinChangeset { get { return checkinChangeset; } set { checkinChangeset = value; } }
		private string checkinChangeset;

		protected FileItem() { }

		public static FileItem CreateFileItem(string filename)
		{
			return new FileItem {
			Filename = filename,
			CheckinChangeset = String.Empty
			};
		}
	}
}
