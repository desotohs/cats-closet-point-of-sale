using System;
using System.Collections.Generic;
using System.Linq;
using CatsCloset.Model.Requests;

namespace CatsCloset.Apis {
	public class OptionList : AbstractApi<EmptyRequest, IEnumerable<KeyValuePair<string, string>>> {
		protected override IEnumerable<KeyValuePair<string, string>> Handle(EmptyRequest req) {
			RequireAuthentication();
			lock ( Context ) {
				return Context.Options
					.ToArray()
					.Select(
						o => new KeyValuePair<string, string>(
							o.Key,
							o.Value));
			}
		}

		public OptionList() : base("/options") {
		}
	}
}

