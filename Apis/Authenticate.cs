using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class Authenticate : AbstractApi<AuthenticateRequest, TokenResponse> {
		protected override TokenResponse Handle(AuthenticateRequest req) {
			User user = Context.Users
				.FirstOrDefault(
					u => u.Username == req.username);
			AccessRequire(user != null);
			byte[] givenHash = HashAlgorithm.Create("sha512")
				.ComputeHash(
					Encoding.UTF8.GetBytes(
						req.password)
					.Concat(
						user.Salt)
					.ToArray());
			AccessRequire(givenHash.SequenceEqual(user.PasswordHash));
			RandomNumberGenerator rng = RandomNumberGenerator.Create();
			byte[] token = new byte[256];
			rng.GetBytes(token);
			string strToken = Convert.ToBase64String(token);
			user.Token = strToken;
			Context.SaveChanges();
			return new TokenResponse(strToken);
		}

		public Authenticate() : base("/authenticate") {
		}
	}
}

