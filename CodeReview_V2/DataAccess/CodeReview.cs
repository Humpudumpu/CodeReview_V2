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
		TFSAccess tf;

		public CodeReview()
		{
			tt = new TeamTrackAccess();
			tf = new TFSAccess();
		}

		public Incident GetIncident(uint incidentNo)
		{
			return (tt.GetIncident(incidentNo));
		public void GetFileDifference(string filename, string checkoutChangeset, string checkinChangeset)
		{
			string argument = String.Format("difference {0} /version:C{1}~C{2} /format:visual", filename, checkoutChangeset, checkinChangeset);
			tf.RunTF<int>(argument, false, -1);
		}
	}
}
