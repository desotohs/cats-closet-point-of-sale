using System;
using System.Linq;
using CatsCloset.Emails;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class Deposit : AbstractApi<DepositRequest, StatusResponse> {
		protected override StatusResponse Handle(DepositRequest req) {
			User user = RequireAuthentication();
			AccessRequire(user.OfficeAccess);
			lock ( Context ) {
				Customer customer = Context.Customers
				.First(
					c => c.Barcode ==
                   req.barcode);
				customer.Balance += req.amount;
				History history = new History();
				history.BalanceChange = req.amount;
				history.Customer = customer;
				history.Time = DateTime.Now;
				history.User = user;
				Context.History.Add(history);
				customer.SendDepositEmail(history);
				Context.SaveChanges();
			}
			return new StatusResponse(true);
		}

		public Deposit() : base("/deposit") {
		}
	}
}

