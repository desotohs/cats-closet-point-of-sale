using System;
using FastCGI;
using Newtonsoft.Json;
using CatsCloset.Model;

namespace CatsCloset.Apis {
	public abstract class AbstractApi<TReq, TRes> : IApi {
		private readonly string Url;

		public Context Context {
			protected get;
			set;
		}

		public virtual bool this[string url] {
			get {
				return Url.Equals(url);
			}
		}

		public bool this[Request req] {
			get {
				if ( this[req.GetParameterUTF8("REQUEST_URI")] ) {
					Handle(req);
					return true;
				} else {
					return false;
				}
			}
		}

		protected abstract TRes Handle(TReq req);

		protected virtual void Handle(Request req) {
			string res;
			if ( req.GetParameterUTF8("REQUEST_METHOD") == "OPTIONS" ) {
				res = "";
			} else {
				TReq reqObj = JsonConvert.DeserializeObject<TReq>(req.GetBody(null));
				TRes resObj = Handle(reqObj);
				res = JsonConvert.SerializeObject(resObj);
			}
			req.WriteResponseASCII("HTTP/1.1 200 OK\nContent-Type: text/json\nAccess-Control-Allow-Origin: *\nAccess-Control-Allow-Headers: DNT,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Pragma\n\n");
			req.WriteResponseUtf8(res);
		}

		protected AbstractApi(string url) {
			Url = url;
		}
	}
}

