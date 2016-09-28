using System;
using FastCGI;
using CatsCloset.Model;

namespace CatsCloset.Apis {
	public interface IApi {
		Context Context {
			set;
		}

		bool this[string url] {
			get;
		}

		bool this[Request req] {
			get;
		}
	}
}

