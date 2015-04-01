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


		public CodeReview()
		{
			tt = new TeamTrackAccess();
		}

		public Incident GetIncident(uint incidentNo)
		{
			return (tt.GetIncident(incidentNo));
		}

		private List<FileItem> GetDisplayFileList(List<CustomChangeset> changesets)
		{
			List<FileItem> tempFileList = new List<FileItem>();
			foreach (CustomChangeset x in changesets)
			{
				foreach (FileItem y in x.Files)
				{
					tempFileList.Add(FileItem.CreateFileItem(y.Filename, x.CheckinChangeSet));
				}
			}
			return tempFileList;
		}
	}
}
