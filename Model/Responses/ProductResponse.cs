using System;

namespace CatsCloset.Model.Responses {
	public class ProductResponse {
		public int id;
		public string name;
		public string desc;
		public string picture;
		public double price;
		public bool enabled;
		public string category;

		public ProductResponse(Product product) {
			id = product.Id;
			name = product.Name;
			desc = product.Description;
			picture = "/image/" + product.ImageId;
			price = product.Price;
			enabled = product.Enabled;
			category = product.Category;
		}
	}
}

