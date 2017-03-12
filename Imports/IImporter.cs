using System;
using ICSharpCode.SharpZipLib.Zip;
using CatsCloset.Model;

namespace CatsCloset.Imports {
	public interface IImporter {
		int Id {
			get;
		}

		int TotalSteps {
			get;
		}

		int CompleteSteps {
			get;
		}

		void Run(Context ctx);

		string Init(ZipFile zip);
	}
}

