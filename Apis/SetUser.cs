using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SetUser : AbstractApi<SaveUserRequest, StatusResponse> {
		protected override StatusResponse Handle(SaveUserRequest req) {
			AccessRequire(RequireAuthentication().SettingsAccess);
			lock ( Context ) {
				User user = Context.Users
					.FirstOrDefault(
						u => u.Id == req.id);
				if ( user == null ) {
					user = new User();
					RandomNumberGenerator rng = RandomNumberGenerator.Create();
					user.Salt = new byte[256];
					rng.GetBytes(user.Salt);
					Context.Users.Add(user);
				}
				user.OfficeAccess = req.officeAccess;
				user.SettingsAccess = req.settingsAccess;
				user.StoreAccess = req.storeAccess;
				user.Username = req.username;
				if ( !string.IsNullOrEmpty(req.password) ) {
					user.PasswordHash = HashAlgorithm.Create("sha512")
						.ComputeHash(
							Encoding.UTF8.GetBytes(
								req.password)
							.Concat(
								user.Salt)
							.ToArray());
				}
				if ( req.invalidateToken ) {
					user.Token = null;
				}
				Context.SaveChanges();
				return new StatusResponse(true);
			}
		}

		public SetUser() : base("/user/save") {
		}
	}
}

