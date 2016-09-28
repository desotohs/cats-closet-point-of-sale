using System;
using System.Collections.Generic;

namespace CatsCloset.Model.Requests {
	public class PurchaseRequest {
		public string barcode;
		public IEnumerable<int> purchases;
	}
}

