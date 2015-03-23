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
		private Incident incident;
		private TFSAccess tf;

		public CodeReview()
		{
			incident = new Incident();
			tf = new TFSAccess();
		}
	}
}
