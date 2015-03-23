using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReview_V2.Model
{
	public class Changeset
	{
		public List<FileItem> Files { get { return files; } set { files = value; } }
		private List<FileItem> files;

		public string DevBranch { get { return devBranch; } set { devBranch = value; } }
		private string devBranch;

		public Changeset()
		{
			Files = new List<FileItem>();
			DevBranch = String.Empty;
		}
	}
}
