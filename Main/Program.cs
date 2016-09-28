using System;
using System.Linq;
using System.Net;
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

		public static void Main(string[] args) {
			ctx = new Context();
			ctx.Database.CreateIfNotExists();
			ctx.Database.Initialize(false);
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

