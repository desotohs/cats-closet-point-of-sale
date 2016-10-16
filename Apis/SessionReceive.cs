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
			if ( req.Method != HttpMethod.Options ) {
				Task<string> task = req.Content.ReadAsStringAsync();
				task.Wait();
				SessionRequest reqObj = JsonConvert.DeserializeObject<SessionRequest>(task.Result);
				SessionMessage msg;
				lock ( Context ) {
					msg = Context.SessionMessages
					.FirstOrDefault(
	                    m => m.Id == reqObj.session);
					if ( msg == null ) {
						msg = new SessionMessage();
						msg.Id = reqObj.session;
						msg.Content = null;
						Context.SessionMessages.Add(msg);
						Context.SaveChanges();
					}
				}
				while ( msg.Content == null ) {
					Thread.Sleep(10);
				}
				res.Content = new StringContent(msg.Content);
				res.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("text/json");
				lock ( Context ) {
					msg.Content = null;
					Context.SaveChanges();
				}
			}
		}

		public SessionReceive() : base("/session/recv") {
		}
	}
}

