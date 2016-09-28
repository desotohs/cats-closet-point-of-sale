using System;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class GetCustomer : AbstractApi<BarcodeRequest, CustomerResponse> {
		protected override CustomerResponse Handle(BarcodeRequest req) {
			Customer customer = Context.Customers
				.FirstOrDefault(
				c => c.Barcode == req.barcode);
			return customer == null ?
				null :
				new CustomerResponse(customer);
		}

		public GetCustomer() : base("/customer") {
		}
	}
}

