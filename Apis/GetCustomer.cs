using System;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class GetCustomer : AbstractApi<CustomerRequest, CustomerResponse> {
		protected override CustomerResponse Handle(CustomerRequest req) {
			User user = RequireAuthentication();
			Customer customer;
			lock ( Context ) {
				customer = Context.Customers
				.FirstOrDefault(
					c => c.Barcode == req.barcode ||
					c.Name == req.name);
			}
			return customer == null ?
				null :
				new CustomerResponse(customer, user.SettingsAccess);
		}

		public GetCustomer() : base("/customer") {
		}
	}
}

