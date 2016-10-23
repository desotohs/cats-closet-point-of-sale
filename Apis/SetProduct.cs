using System;
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
					Context.Products.Add(product);
				}
				product.Description = req.desc;
				product.Enabled = req.enabled;
				product.Name = req.name;
				product.Price = req.price;
				product.Category = req.category;
				Context.SaveChanges();
			}
			return new StatusResponse(true);
		}

		public SetProduct() : base("/product/save") {
		}
	}
}

