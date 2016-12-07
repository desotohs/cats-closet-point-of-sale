using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
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
				} catch ( DbEntityValidationException ex ) {
					foreach ( DbEntityValidationResult result in ex.EntityValidationErrors ) {
						foreach ( DbValidationError error in result.ValidationErrors ) {
							Console.WriteLine("Error: {2} in {0}.{1}", result.Entry.Entity.GetType().FullName, error.PropertyName, error.ErrorMessage);
						}
					}
				} catch ( InvalidOperationException ex ) {
					if ( ex.Message == "Unexpected connection state. When using a wrapping provider ensure that the StateChange event is implemented on the wrapped DbConnection." ) {
						try {
							return ApiFactory.HandleRequest(request, ctx);
						} catch ( Exception ex2 ) {
							Console.Error.WriteLine(ex2);
						}
					} else {
						Console.Error.WriteLine(ex);
					}
				} catch ( Exception ex ) {
					Console.Error.WriteLine(ex);
				}
				return new HttpResponseMessage(HttpStatusCode.InternalServerError);
			});
		}

		public void Configuration(IAppBuilder appBuilder) {
			HttpConfiguration config = new HttpConfiguration();
            config.EnableSystemDiagnosticsTracing().MinimumLevel = TraceLevel.Warn;
			config.Routes.MapHttpRoute("APIs", "", null, null, this);
			config.MessageHandlers.Add(this);
			config.EnsureInitialized();
			appBuilder.UseWebApi(config);
		}

        public static void RunServer(bool interactive) {
            ctx = new Context();
			ctx.Database.CreateIfNotExists();
			ctx.Database.Initialize(false);
			EnsureUserExists();
			SessionMonitor monitor = new SessionMonitor(ctx);
			monitor.PurgeSessions();
			EmailSystem.Init(ctx);
			using ( WebApp.Start<Program>("http://+:8888/") ) {
				monitor.Start();
				Console.WriteLine("Application started.");
				if ( interactive ) {
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

		public static void Main(string[] args) {
			if ( args.Length > 0 ) {
                switch ( args[0] ) {
                    case "launch":
                        RunServer(true);
                        break;
                    default:
                        ServiceBase.Run(new Service());
                        break;
                }
            } else {
                ServiceBase.Run(new Service());
            }
		}
	}
}

