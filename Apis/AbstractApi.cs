using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using CatsCloset.Model;

namespace CatsCloset.Apis {
	public abstract class AbstractApi<TReq, TRes> : IApi {
		private readonly object Lock;
		private HttpRequestMessage CurrentRequest;
		protected readonly string Url;

		protected Context Context {
			get;
			private set;
		}

		public virtual bool this[string url] {
			get {
				return Url.Equals(url);
			}
		}

		public bool this[HttpRequestMessage req, HttpResponseMessage res] {
			get {
				if ( this[req.RequestUri.AbsolutePath] ) {
					Handle(req, res);
					return true;
				} else {
					return false;
				}
			}
		}

		protected void AccessRequire(bool condition) {
			if ( !condition ) {
				throw new UnauthorizedAccessException();
			}
		}

		protected User RequireAuthentication() {
			string token = CurrentRequest.Headers.GetValues("X-Auth-Token").FirstOrDefault();
			AccessRequire(token != null);
			User user;
			lock ( Context ) {
				user = Context.Users
					.FirstOrDefault(
						u => u.Token == token);
			}
			AccessRequire(user != null);
			return user;
		}

		protected abstract TRes Handle(TReq req);

		protected virtual void AddHeaders(HttpResponseMessage res) {
			res.Headers.Add("Access-Control-Allow-Origin", "*");
			res.Headers.Add("Access-Control-Allow-Headers", "DNT,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Pragma,X-Auth-Token");
		}

		protected virtual void Handle(HttpRequestMessage req, HttpResponseMessage res) {
			AddHeaders(res);
			string resStr;
			if ( req.Method == HttpMethod.Options ) {
				resStr = "";
			} else {
				Task<string> contentTask = req.Content.ReadAsStringAsync();
				contentTask.Wait();
				TReq reqObj = JsonConvert.DeserializeObject<TReq>(contentTask.Result);
				try {
					lock ( Lock ) {
						CurrentRequest = req;
						using (Context = new Context()) {
							resStr = JsonConvert.SerializeObject(Handle(reqObj));
						}
					}
				} catch ( UnauthorizedAccessException ) {
					res.StatusCode = HttpStatusCode.Forbidden;
					res.Content = new StringContent("{\"unauthorized\":true}");
					return;
				}
			}
			res.Content = new StringContent(resStr);
			res.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
		}

		protected AbstractApi(string url) {
			Url = url;
			Lock = new object();
		}
	}
}

