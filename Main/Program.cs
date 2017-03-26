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
		private const int MaxTries = 5;
		private const int RetryTimeout = 10;
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
				Console.WriteLine(request.RequestUri);
				Exception[] exs = new Exception[MaxTries];
				for ( int i = 0; i < MaxTries; ++i ) {
					try {
						return ApiFactory.HandleRequest(request, ctx);
					} catch ( DbEntityValidationException ex ) {
						foreach ( DbEntityValidationResult result in ex.EntityValidationErrors ) {
							foreach ( DbValidationError error in result.ValidationErrors ) {
								Console.WriteLine("Error: {2} in {0}.{1}", result.Entry.Entity.GetType().FullName, error.PropertyName, error.ErrorMessage);
							}
						}
					} catch ( Exception ex ) {
						exs[i] = ex;
						Thread.Sleep(RetryTimeout);
					}
				}
				Console.Error.WriteLine("Request for {0} crashed {1} times; max. retries failed.", request.RequestUri, MaxTries);
				foreach ( Exception ex in exs ) {
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
			while (true) {
				try {
					ctx = new Context();
					ctx.Database.CreateIfNotExists();
					ctx.Database.Initialize(false);
					break;
				} catch (Exception ex) {
					Console.WriteLine(ex);
				}
			}
			EnsureUserExists();
			SessionMonitor monitor = new SessionMonitor(ctx);
			monitor.PurgeSessions();
			EmailSystem.Init(ctx);
			PurchaseReporter reporter = new PurchaseReporter(ctx);
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
			reporter.Stop();
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

