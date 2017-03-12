using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using CatsCloset.Model;

namespace CatsCloset.Imports {
	public static class ImportSystem {
		private static IImporter[] Importers = Assembly.GetExecutingAssembly()
			.GetTypes()
			.Where(
				t => typeof(IImporter).IsAssignableFrom(t) &&
				!t.IsInterface &&
				!t.IsAbstract)
			.Select(
				t => t.GetConstructor(new Type[0])
				.Invoke(new object[0]))
			.Cast<IImporter>()
			.ToArray();

		public static string StartImport(byte[] data, int id, Context ctx, out IImporter importer) {
			IImporter _importer = importer = Importers.FirstOrDefault(i => i.Id == id);
			if (importer == null) {
				return "Unknown import type";
			} else {
				if (importer.CompleteSteps != importer.TotalSteps) {
					return "Import already running";
				}
				MemoryStream stream = new MemoryStream(data, false);
				ZipFile file = new ZipFile(stream);
				string err = importer.Init(file);
				Task.Run(() => {
					_importer.Run(ctx);
				});
				return err;
			}
		}
	}
}

