using System;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class Deposit : AbstractApi<DepositRequest, StatusResponse> {
		protected override StatusResponse Handle(DepositRequest req) {
			AccessRequire(RequireAuthentication().OfficeAccess);
			lock ( Context ) {
				Customer customer = Context.Customers
				.First(
					c => c.Barcode ==
                   req.barcode);
				customer.Balance += req.amount;
				Context.SaveChanges();
			}
			return new StatusResponse(true);
		}

		public Deposit() : base("/deposit") {
		}
	}
}

