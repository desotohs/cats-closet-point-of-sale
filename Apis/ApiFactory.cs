using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using CatsCloset.Model;

namespace CatsCloset.Apis {
	public static class ApiFactory {
		private static List<IApi> Apis;

		private static void PopulateApiList() {
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
		}

		private static void EnsureApiListPopulated() {
			if ( Apis == null ) {
				PopulateApiList();
			}
		}

		public static HttpResponseMessage HandleRequest(HttpRequestMessage msg) {
			EnsureApiListPopulated();
			HttpResponseMessage res = new HttpResponseMessage(HttpStatusCode.OK);
			if ( !Apis.Any(a => {
				return a[msg, res];
			}) ) {
				return new HttpResponseMessage(HttpStatusCode.NotFound);
			}
			return res;
		}
	}
}

