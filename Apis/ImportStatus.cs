using System;
using CatsCloset.Imports;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class ImportStatus : AbstractApi<string, ImportResponse> {
		protected override ImportResponse Handle(string req) {
			//AccessRequire(RequireAuthentication().SettingsAccess);
			IImporter importer = StartImport.GetImporter(req);
			return new ImportResponse(req, importer.TotalSteps, importer.CompleteSteps);
		}

		public ImportStatus() : base("/import/status") {
		}
	}
}

