using System;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class GetOption : AbstractApi<OptionRequest, KeyValuePair> {
		protected override KeyValuePair Handle(OptionRequest req) {
			RequireAuthentication();
			Option opt;
			lock ( Context ) {
				opt = Context.Options
					.First(
			            o => o.Key == req.name);
			}
			return new KeyValuePair(opt.Key, opt.Value);
		}

		public GetOption() : base("/option") {
		}
	}
}

