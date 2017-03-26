using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatsCloset.Model {
	public class Product {
		[Key]
		public int Id {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}

		[ForeignKey("Image")]
		public int ImageId {
			get;
			set;
		}

		public virtual Image Image {
			get;
			set;
		}

		public int Price {
			get;
			set;
		}

		public bool Enabled {
			get;
			set;
		}

		public string Category {
			get;
			set;
		}
	}
}

