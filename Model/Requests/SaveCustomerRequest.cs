using System;
using System.Collections.Generic;
using CatsCloset.Model.Responses;

namespace CatsCloset.Model.Requests {
	public class SaveCustomerRequest {
		public string name;
		public string code;
		public double balance;
		public string pin;
		public List<KeyValuePair> properties;
	}
}

