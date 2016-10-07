using System;
using System.Linq;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SessionReceive : AbstractApi<SessionRequest, SessionDataResponse> {
		protected override SessionDataResponse Handle(SessionRequest req) {
			return new SessionDataResponse(Context.SessionMessages
				.First(
					m => m.Id == req.session));
		}

		public SessionReceive() : base("/session/recv") {
		}
	}
}

