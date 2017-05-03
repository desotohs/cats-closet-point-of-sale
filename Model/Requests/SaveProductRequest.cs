using System;

namespace CatsCloset.Model.Requests {
	public class SaveProductRequest {
		public int id;
		public string name;
		public string desc;
		public double price;
		public bool enabled;
		public string category;
		public int inventory;
	}
}

