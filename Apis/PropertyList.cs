using System;
using System.Collections.Generic;
using System.Linq;
using CatsCloset.Model.Requests;

namespace CatsCloset.Apis {
	public class PropertyList : AbstractApi<EmptyRequest, IEnumerable<string>> {
		protected override IEnumerable<string> Handle(EmptyRequest req) {
			RequireAuthentication();
			return Context.CustomProperties
				.Select(
					p => p.Name);
		}

		public PropertyList() : base("/properties") {
		}
	}
}

