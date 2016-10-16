using System;
using System.Net.Http;
using CatsCloset.Model;

namespace CatsCloset.Apis {
	public interface IApi {
		Context Context {
			set;
		}

		bool this[string url] {
			get;
		}

		bool this[HttpRequestMessage req, HttpResponseMessage res] {
			get;
		}
	}
}

