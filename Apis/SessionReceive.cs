using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SessionReceive : AbstractApi<SessionRequest, SessionDataResponse> {
		protected override SessionDataResponse Handle(SessionRequest req) {
			return null;
		}

		protected override void Handle(HttpRequestMessage req, HttpResponseMessage res) {
			AddHeaders(res);
			if (req.Method != HttpMethod.Options) {
				Task<string> task = req.Content.ReadAsStringAsync();
				task.Wait();
				SessionRequest reqObj = JsonConvert.DeserializeObject<SessionRequest>(task.Result);
				SessionMessage msg;
				using (Context ctx = new Context()) {
					msg = ctx.SessionMessages
						.FirstOrDefault(
		                    m => m.Id == reqObj.session);
					if (msg == null || msg.Content == null) {
						Thread thread = Thread.CurrentThread;
						Action<SessionSendEvent> action = ev => {
							if (ev.SessionId == reqObj.session) {
								res.Content = new StringContent(ev.Data);
								ev.PreventDefault();
								thread.Interrupt();
							}
						};
						SessionSend.Sent += action;
						try {
							while (true) {
								Thread.Sleep(int.MaxValue);
							}
						} catch {
						} finally {
							SessionSend.Sent -= action;
						}
					} else {
						res.Content = new StringContent(msg.Content);
						msg.Content = null;
						ctx.SaveChanges();
					}
				}
				res.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
			}
		}

		public SessionReceive() : base("/session/recv") {
		}
	}
}

