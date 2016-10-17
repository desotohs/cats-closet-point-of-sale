using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using CatsCloset.Apis;
using CatsCloset.Emails;
using CatsCloset.Model;

namespace CatsCloset.Main {
	public class Program : DelegatingHandler {
		private static Context ctx;

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

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
			return Task.Run(() => {
				try {
					Console.WriteLine(request.RequestUri);
					return ApiFactory.HandleRequest(request, ctx);
				} catch ( Exception ex ) {
					Console.Error.WriteLine(ex);
					if ( ex is DbEntityValidationException ) {
						foreach ( DbEntityValidationResult result in ((DbEntityValidationException) ex).EntityValidationErrors ) {
							foreach ( DbValidationError error in result.ValidationErrors ) {
								Console.WriteLine("Error: {2} in {0}.{1}", result.Entry.Entity.GetType().FullName, error.PropertyName, error.ErrorMessage);
							}
						}
					}
					return new HttpResponseMessage(HttpStatusCode.InternalServerError);
				}
			});
		}



		public void Configuration(IAppBuilder appBuilder) {
			HttpConfiguration config = new HttpConfiguration();
			config.Routes.MapHttpRoute("APIs", "", null, null, this);
			config.MessageHandlers.Add(this);
			config.EnsureInitialized();
			appBuilder.UseWebApi(config);
		}

		public static void Main(string[] args) {
			ctx = new Context();
			ctx.Database.CreateIfNotExists();
			ctx.Database.Initialize(false);
			EnsureUserExists();
			SessionMonitor monitor = new SessionMonitor(ctx);
			monitor.PurgeSessions();
			EmailSystem.Init(ctx);
			using ( WebApp.Start<Program>("http://*:8080") ) {
				monitor.Start();
				Console.WriteLine("Application started.");
				if ( Environment.UserInteractive ) {
					Console.WriteLine("Press any key to stop the server");
					Console.ReadKey();
				} else {
					try {
						while ( true ) {
							Thread.Sleep(int.MaxValue);
						}
					} catch ( Exception ex ) {
						Console.Error.WriteLine(ex);
					}
				}
				Console.WriteLine("Shutting down server...");
			}
		}
	}
}

