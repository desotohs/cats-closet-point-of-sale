using System;

namespace CatsCloset.Model.Responses {
	public class SessionDataResponse {
		public string session;
		public string data;

		public SessionDataResponse(SessionMessage msg) {
			session = msg.Id;
			data = msg.Content;
		}
	}
}

