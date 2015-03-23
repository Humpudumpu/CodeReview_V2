using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeReview_V2.DataAccess
{
	public class TeamTrackAccess
	{
		TeamTrack tt;
		bool LoggedIn { get; set; }
		public TeamTrackAccess()
		{
			tt = new TeamTrack();
			LoggedIn = false;
		}
	}
}
