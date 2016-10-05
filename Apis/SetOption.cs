using System;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SetOption : AbstractApi<KeyValuePair, StatusResponse> {
		protected override StatusResponse Handle(KeyValuePair req) {
			Option opt = Context.Options
				.FirstOrDefault(
					o => o.Key == req.name);
			if ( opt == null ) {
				opt = new Option();
				opt.Key = req.name;
				Context.Options.Add(opt);
			}
			opt.Value = req.value;
			Context.SaveChanges();
			return new StatusResponse(true);
		}

		public SetOption() : base("/option/save") {
		}
	}
}

