using System;
using System.Collections.Generic;

namespace CatsCloset.Model.Requests {
	public class PurchaseRequest {
		public string barcode;
		public string pin;
		public IEnumerable<int> purchases;
	}
}

