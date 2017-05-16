using System;
using System.Net.Http;

namespace CatsCloset.Apis {
	public interface IApi {
		bool this[string url] {
			get;
		}

		bool this[HttpRequestMessage req, HttpResponseMessage res] {
			get;
		}
	}
}

