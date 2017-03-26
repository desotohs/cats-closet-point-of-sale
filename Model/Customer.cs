using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatsCloset.Model {
	public class Customer {
		[Key]
		public int Id {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public int Balance {
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

		public string Barcode {
			get;
			set;
		}

		public string Pin {
			get;
			set;
		}

		public virtual List<CustomerProperty> Properties {
			get;
			set;
		}

		public virtual List<History> PurchaseHistory {
			get;
			set;
		}
	}
}

