using System;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class Purchase : AbstractApi<PurchaseRequest, StatusResponse> {
		protected override StatusResponse Handle(PurchaseRequest req) {
			AccessRequire(RequireAuthentication().StoreAccess);
			double cost = req.purchases
				.Select(
					i => Context.Products
					.First(
						p => p.Id == i)
					.Price)
				.Sum();
			Customer customer = Context.Customers
				.First(
					c => c.Barcode ==
					req.barcode);
			if ( customer.Balance >= cost ) {
				customer.Balance -= cost;
				Context.SaveChanges();
				return new StatusResponse(true);
			} else {
				return new StatusResponse(false);
			}
		}

		public Purchase() : base("/purchase") {
		}
	}
}

