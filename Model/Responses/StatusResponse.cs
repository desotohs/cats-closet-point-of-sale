using System;

namespace CatsCloset.Model.Responses {
	public class StatusResponse {
		public bool success;

		public StatusResponse(bool success) {
			this.success = success;
		}
	}
}

