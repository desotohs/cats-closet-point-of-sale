using System;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SessionSend : AbstractApi<SessionRequest, StatusResponse> {
		public static event Action<SessionSendEvent> Sent;

		protected override StatusResponse Handle(SessionRequest req) {
			SessionSendEvent ev = new SessionSendEvent(req.session, req.data);
			if (Sent != null) {
				Sent(ev);
			}
			if (!ev.DefaultPrevented) {
				using (Context ctx = new Context()) {
					SessionMessage msg = ctx.SessionMessages
						.FirstOrDefault(
							m => m.Id == req.session);
					if (msg == null) {
						msg = new SessionMessage();
						msg.Id = req.session;
						msg.LastUpdate = DateTime.Now;
						ctx.SessionMessages.Add(msg);
					}
					msg.Content = req.data;
					msg.LastUpdate = DateTime.Now;
					ctx.SaveChanges();
				}
			}
			return new StatusResponse(true);
		}

		public SessionSend() : base("/session/send") {
		}
	}
}

