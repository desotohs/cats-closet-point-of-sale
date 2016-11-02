using System;
using System.Collections.Generic;
using System.Linq;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class EnabledProducts : AbstractApi<EmptyRequest, IEnumerable<ProductResponse>> {
		protected override IEnumerable<ProductResponse> Handle(EmptyRequest req) {
			RequireAuthentication();
			lock ( Context ) {
				return Context.Products
					.Where(
						p => p.Enabled)
					.ToArray()
					.Select(
						p => new ProductResponse(p, Context));
			}
		}

		public EnabledProducts() : base("/products/enabled") {
		}
	}
}

