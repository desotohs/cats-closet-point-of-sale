using System;
using System.IO;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SetProduct : AbstractApi<SaveProductRequest, StatusResponse> {
		protected override StatusResponse Handle(SaveProductRequest req) {
			AccessRequire(RequireAuthentication().SettingsAccess);
			lock ( Context ) {
				Product product = Context.Products
					.FirstOrDefault(
		                 p => p.Id == req.id);
				if ( product == null ) {
					product = new Product();
					Image image = new Image();
					image.Data = File.ReadAllBytes("Assets/default-product.png");
					Context.Images.Add(image);
					product.Image = image;
					Context.Products.Add(product);
				}
				product.Description = req.desc;
				product.Enabled = req.enabled;
				product.Name = req.name;
				product.Price = (int) (req.price * 100);
				product.Category = req.category;
				product.InventoryAmount = req.inventory;
				Context.SaveChanges();
			}
			return new StatusResponse(true);
		}

		public SetProduct() : base("/product/save") {
		}
	}
}

