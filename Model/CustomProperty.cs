using System;
using System.ComponentModel.DataAnnotations;

namespace CatsCloset.Model {
	public class CustomProperty {
		[Key]
		public int Id {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}
	}
}

