using System;
using System.ComponentModel.DataAnnotations;

namespace CatsCloset.Model {
	public class Option {
		[Key]
		public string Key {
			get;
			set;
		}

		public string Value {
			get;
			set;
		}
	}
}

