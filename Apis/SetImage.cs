using System;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SetImage : AbstractApi<ImageRequest, StatusResponse> {
		protected override StatusResponse Handle(ImageRequest req) {
			AccessRequire(RequireAuthentication().SettingsAccess);
			Image image = new Image();
			image.Data = Convert.FromBase64String(req.data);
			lock ( Context ) {
				Context.Images.Add(image);
				foreach ( string customerName in req.customerNames ) {
					Context.Customers
						.First(
							c => c.Name == customerName)
						.Image = image;
				}
				foreach ( int productId in req.productIds ) {
					Context.Products
						.First(
							p => p.Id == productId)
						.Image = image;
				}
				Context.SaveChanges();
			}
			return new StatusResponse(true);
		}

		public SetImage() : base("/image/save") {
		}
	}
}

