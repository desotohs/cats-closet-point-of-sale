using System;
using System.Threading;
using ICSharpCode.SharpZipLib.Zip;
using CatsCloset.Model;

namespace CatsCloset.Imports {
	public class TestImporter : IImporter {
		public int Id {
			get {
				return 0;
			}
		}

		public int TotalSteps {
			get;
			set;
		}

		public int CompleteSteps {
			get;
			set;
		}

		public void Run(Context ctx) {
			for (; CompleteSteps < TotalSteps; ++CompleteSteps) {
				Thread.Sleep(100);
			}
		}

		public string Init(ZipFile zip) {
			TotalSteps = 300;
			CompleteSteps = 0;
			return null;
		}
	}
}

