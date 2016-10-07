using System;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SessionSend : AbstractApi<SessionRequest, StatusResponse> {
		protected override StatusResponse Handle(SessionRequest req) {
			SessionMessage msg = Context.SessionMessages
				.FirstOrDefault(
					m => m.Id == req.session);
			if ( msg == null ) {
				msg = new SessionMessage();
				msg.Id = req.session;
				Context.SessionMessages.Add(msg);
			}
			msg.Content = req.data;
			Context.SaveChanges();
			return new StatusResponse(true);
		}

		public SessionSend() : base("/session/send") {
		}
	}
}

