using System;
using System.Linq;
using FastCGI;

namespace CatsCloset.Apis {
	public class Image : AbstractApi<int, byte[]> {
		public override bool this[string url] {
			get {
				return url.StartsWith(Url);
			}
		}

		protected override byte[] Handle(int req) {
			return Context.Images
				.First(
					i => i.Id == req)
				.Data;
		}

		protected override void Handle(Request req) {
			if ( req.GetParameterUTF8("REQUEST_METHOD") == "OPTIONS" ) {
				req.WriteResponseASCII("HTTP/1.1 200 OK\nContent-Type: text/json\nAccess-Control-Allow-Origin: *\nAccess-Control-Allow-Headers: DNT,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Pragma\n\n");
			} else {
				int reqObj = int.Parse(req.GetParameterUTF8("REQUEST_URI").Substring(Url.Length));
				byte[] res = Handle(reqObj);
				req.WriteResponseASCII("HTTP/1.1 200 OK\nAccess-Control-Allow-Origin: *\nAccess-Control-Allow-Headers: DNT,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Pragma\nContent-Type: ");
				req.WriteResponseASCII(Magic.GetMime(res));
				req.WriteResponseASCII("\nContent-Length: ");
				req.WriteResponseASCII(res.Length.ToString());
				req.WriteResponseASCII("\n\n");
				req.WriteResponse(res);
			}
		}

		public Image() : base("/image/") {
		}
	}
}

