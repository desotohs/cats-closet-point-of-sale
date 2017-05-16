using System;
using System.Collections.Generic;
using CatsCloset.Imports;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class StartImport : AbstractApi<ImportRequest, ImportResponse> {
		private static readonly Dictionary<string, IImporter> Importers = new Dictionary<string, IImporter>();

		protected override ImportResponse Handle(ImportRequest req) {
			AccessRequire(RequireAuthentication().SettingsAccess);
			IImporter importer;
			string error = ImportSystem.StartImport(Convert.FromBase64String(req.data), req.type, out importer);
			if (error != null) {
				return new ImportResponse(error);
			}
			byte[] bytes = new byte[16];
			new Random().NextBytes(bytes);
			string token = Convert.ToBase64String(bytes);
			Importers[token] = importer;
			return new ImportResponse(token, importer.TotalSteps, importer.CompleteSteps);
		}

		public static IImporter GetImporter(string id) {
			return Importers[id];
		}

		public StartImport() : base("/import/start") {
		}
	}
}

