using System;
using System.Collections.Generic;
using System.Linq;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class ProductList : AbstractApi<EmptyRequest, IEnumerable<ProductResponse>> {
		protected override IEnumerable<ProductResponse> Handle(EmptyRequest req) {
			RequireAuthentication();
			lock ( Context ) {
				return Context.Products
					.ToArray()
					.Select(
						p => new ProductResponse(p));
			}
		}

		public ProductList() : base("/products") {
		}
	}
}

