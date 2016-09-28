using System;
using System.Collections.Generic;
using System.Linq;
using CatsCloset.Model.Requests;

namespace CatsCloset.Apis {
	public class CustomerList : AbstractApi<EmptyRequest, IEnumerable<string>> {
		protected override IEnumerable<string> Handle(EmptyRequest req) {
			return Context.Customers.Select(c => c.Name);
		}

		public CustomerList() : base("/customers") {
		}
	}
}

