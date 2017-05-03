using System;
using System.Linq;

namespace CatsCloset.Model.Responses {
	public class ProductResponse {
		public int id;
		public string name;
		public string desc;
		public string picture;
		public double price;
		public bool enabled;
		public string category;
		public int inventory;

		public ProductResponse(Product product, Context context, bool includeTax) {
			id = product.Id;
			name = product.Name;
			desc = product.Description;
			picture = "/image/" + product.ImageId;
			price = product.Price * (includeTax ? (1 +
				double.Parse((
					context.Options
					.FirstOrDefault(
						o => o.Key == "Tax")
					?? new Option() { Value = "0" })
					.Value)) / 100.0 : 0.01);
			enabled = product.Enabled;
			category = product.Category;
			inventory = product.InventoryAmount;
		}
	}
}

