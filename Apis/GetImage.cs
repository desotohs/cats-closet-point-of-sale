using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using CatsCloset.Model;

namespace CatsCloset.Apis {
	public class GetImage : AbstractApi<int, byte[]> {
		public override bool this[string url] {
			get {
				return url.StartsWith(Url);
			}
		}

		protected override byte[] Handle(int req) {
			using (Context ctx = new Context()) {
				return ctx.Images
					.First(
						i => i.Id == req)
					.Data;
			}
		}

		protected override void Handle(HttpRequestMessage req, HttpResponseMessage res) {
			if ( req.Method != HttpMethod.Options ) {
				int reqObj = int.Parse(req.RequestUri.AbsolutePath.Substring(Url.Length));
				byte[] img = Handle(reqObj);
				res.Content = new ByteArrayContent(img == null ? new byte[0] : img);
				res.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(Magic.GetMime(img));
			}
		}

		public GetImage() : base("/image/") {
		}
	}
}

