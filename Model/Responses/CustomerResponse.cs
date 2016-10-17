using System;
using System.Collections.Generic;
using System.Linq;

namespace CatsCloset.Model.Responses {
	public class CustomerResponse {
		public string name;
		public double balance;
		public string picture;
		public string code;
		public string pin;
		public int pinLength;
		public IEnumerable<KeyValuePair> properties;

		public CustomerResponse(Customer customer, bool includePin) {
			name = customer.Name;
			balance = customer.Balance;
			picture = "/image/" + customer.ImageId;
			code = customer.Barcode;
			pin = includePin ? customer.Pin : null;
			pinLength = customer.Pin.Length;
			properties = customer.Properties
				.Select(
					p => new KeyValuePair(
						p.Property.Name,
						p.Value));
		}
	}
}

