using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using CatsCloset.Model;

namespace CatsCloset.Imports {
	public class CustomerImporter : IImporter {
		private ZipFile Zip;
		private ZipEntry Csv;

		public int Id {
			get {
				return 1;
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
			string[] barcodes = ctx.Customers
				.Select(
                    c => c.Barcode)
				.ToArray();
			Image img = ctx.Images
				.FirstOrDefault(
					i => i.Data == null);
			if (img == null) {
				img = new Image();
				ctx.Images.Add(img);
			}
			using (CsvFile csv = new CsvFile(Zip.GetInputStream(Csv))) {
				while (csv.NextLine()) {
					if (!barcodes.Contains(csv["Barcode", null])) {
						lock (ctx) {
							Customer customer = new Customer();
							customer.Barcode = csv["Barcode", ""];
							customer.Balance = (int) (csv["Balance", 0] * 100);
							customer.Name = csv["Name", ""];
							customer.Pin = csv["Pin", "-----"];
							customer.Image = img;
							ctx.Customers.Add(customer);
							foreach (CustomProperty prop in ctx.CustomProperties) {
								CustomerProperty p = new CustomerProperty();
								p.Customer = customer;
								p.Property = prop;
								p.Value = csv[prop.Name, ""];
								ctx.CustomerProperties.Add(p);
							}
							ctx.SaveChanges();
						}
					}
					++CompleteSteps;
				}
			}
		}

		public string Init(ZipFile zip) {
			IEnumerable<ZipEntry> csvs = zip.Cast<ZipEntry>().Where(e => e.Name.EndsWith(".csv"));
			int numCsv = csvs.Count();
			if (numCsv == 0) {
				return "CSV not found.";
			} else if (numCsv > 1) {
				return "Too many CSVs found.";
			}
			Zip = zip;
			Csv = csvs.First();
			CompleteSteps = TotalSteps = 0;
			using (CsvFile csv = new CsvFile(Zip.GetInputStream(Csv))) {
				while (csv.NextLine()) {
					++TotalSteps;
				}
			}
			return null;
		}
	}
}

