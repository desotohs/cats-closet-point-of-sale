using System;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using CatsCloset.Model;

namespace CatsCloset.Imports {
	public class PictureImporter : IImporter {
		private ZipFile Zip;

		public int Id {
			get {
				return 2;
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
			foreach (ZipEntry entry in Zip
				.Cast<ZipEntry>()
				.Where(
					e => e.IsFile &&
					e.Name.EndsWith(".jpg"))) {
				string[] dirs = entry.Name.Split('/');
				string[] parts = dirs[dirs.Length - 1].ToLower().Split('_');
				string name = string.Concat(parts[1], ' ', parts[0]);
				lock (ctx) {
					Customer customer = ctx.Customers
						.FirstOrDefault(
	                   		c => c.Name.ToLower().Equals(name));
					if (customer != null) {
						Image old = customer.Image;
						Image import = new Image();
						import.Data = new byte[entry.Size];
						using (Stream stream = Zip.GetInputStream(entry)) {
							stream.Read(import.Data, 0, import.Data.Length);
						}
						customer.Image = import;
						ctx.SaveChanges();
						if (old.Data != null &&
						   !(ctx.Customers
							.Any(
							   c => c.ImageId == old.Id) ||
						   ctx.Products
							.Any(
							   p => p.ImageId == old.Id))) {
							ctx.Images.Remove(old);
						}
						ctx.SaveChanges();
					}
				}
				++CompleteSteps;
			}
		}

		public string Init(ZipFile zip) {
			Zip = zip;
			TotalSteps = Zip.Cast<ZipEntry>()
				.Where(
					e => e.IsFile)
				.Select(
					e => e.Name)
				.Where(
					s => s.EndsWith(".jpg"))
				.Count();
			CompleteSteps = 0;
			return null;
		}
	}
}

