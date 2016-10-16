using System;
using System.Linq;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class HasSessionResponded : AbstractApi<SessionRequest, StatusResponse> {
		protected override StatusResponse Handle(SessionRequest req) {
			lock ( Context ) {
				return new StatusResponse(Context.SessionMessages
					.Any(
						m => m.Id == req.session &&
						m.Content == null));
			}
		}

		public HasSessionResponded() : base("/session/responded") {
		}
	}
}

