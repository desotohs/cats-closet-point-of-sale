using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FastCGI;
using CatsCloset.Model;

namespace CatsCloset.Apis {
	public static class ApiFactory {
		private static List<IApi> Apis;

		private static void PopulateApiList(Context ctx) {
			Apis = new List<IApi>();
			Apis.AddRange(
				Assembly.GetExecutingAssembly()
				.GetTypes()
				.Where(
					t => typeof(IApi).IsAssignableFrom(t) &&
					!t.IsInterface &&
					!t.IsAbstract)
				.Select(
					t => t.GetConstructor(new Type[0])
					.Invoke(new object[0]))
				.Cast<IApi>());
			Apis.ForEach(a => a.Context = ctx);
		}

		private static void EnsureApiListPopulated(Context ctx) {
			if ( Apis == null ) {
				PopulateApiList(ctx);
			}
		}

		private static void WriteError(Request req) {
			req.WriteResponseASCII("HTTP/1.1 404 Not Found\nContent-Type: text/plain\n\n");
			req.WriteResponseUtf8("The specified URL could not be found.");
		}

		public static void HandleRequest(Request req, Context ctx) {
			EnsureApiListPopulated(ctx);
			if ( !Apis.Any(a => a[req]) ) {
				WriteError(req);
			}
		}
	}
}

