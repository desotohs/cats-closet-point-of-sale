using System;

namespace CatsCloset.Model.Responses {
	public class ImportResponse {
		public string error;
		public string importId;
		public int totalSteps;
		public int completeSteps;

		public ImportResponse(string error) {
			this.error = error;
		}

		public ImportResponse(string importId, int totalSteps, int completeSteps) {
			this.importId = importId;
			this.totalSteps = totalSteps;
			this.completeSteps = completeSteps;
		}
	}
}

