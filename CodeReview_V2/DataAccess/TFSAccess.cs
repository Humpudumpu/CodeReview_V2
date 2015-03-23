using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PVCSTools;
namespace CodeReview_V2.DataAccess
{
	public class TFSAccess
	{
		public string TfsExecutable { get { return tfsExecutable; } set { tfsExecutable = value; } }
		private string tfsExecutable;

		public TFSAccess()
		{
			TfsExecutable = Path.Combine(Environment.GetEnvironmentVariable("VS110COMNTOOLS").Replace("Tools", "IDE"), "tf.exe");
		}
	}
}
