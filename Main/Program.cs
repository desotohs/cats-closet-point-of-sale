using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using FastCGI;
using CatsCloset.Apis;
using CatsCloset.Model;

namespace CatsCloset.Main {
	public static class Program {
		private static Context ctx;

		private static void HandleRequest(object sender, Request e) {
			try {
				Console.WriteLine(e.GetParameterUTF8("REQUEST_URI"));
				ApiFactory.HandleRequest(e, ctx);
				e.Close();
			} catch ( Exception ex ) {
				Console.Error.WriteLine(ex);
			}
		}

		private static void EnsureUserExists() {
			if ( !ctx.Users.Any() ) {
				User user = new User();
				user.Username = "admin";
				RandomNumberGenerator rng = RandomNumberGenerator.Create();
				byte[] salt = new byte[256];
				rng.GetBytes(salt);
				user.Salt = salt;
				user.PasswordHash = HashAlgorithm.Create("sha512")
					.ComputeHash(
						Encoding.UTF8.GetBytes(
							"admin")
						.Concat(
							salt)
						.ToArray());
				user.OfficeAccess = true;
				user.SettingsAccess = true;
				user.StoreAccess = true;
				ctx.Users.Add(user);
				ctx.SaveChanges();
			}
		}

		public static void Main(string[] args) {
			ctx = new Context();
			ctx.Database.CreateIfNotExists();
			ctx.Database.Initialize(false);
			EnsureUserExists();
			FCGIApplication app = new FCGIApplication();
			app.OnRequestReceived += HandleRequest;
			app.Listen(new IPEndPoint(IPAddress.Any, 9000));
			Console.WriteLine("Applications started.");
			while ( !app.IsStopping ) {
				if ( !app.Process() ) {
					Thread.Sleep(1);
				}
			}
		}
	}
}

