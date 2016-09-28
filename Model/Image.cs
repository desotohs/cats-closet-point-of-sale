using System;
using System.ComponentModel.DataAnnotations;

namespace CatsCloset.Model {
	public class Image {
		[Key]
		public int Id {
			get;
			set;
		}

		public byte[] Data {
			get;
			set;
		}
	}
}

