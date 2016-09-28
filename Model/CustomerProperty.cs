using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatsCloset.Model {
	public class CustomerProperty {
		[Key]
		public int Id {
			get;
			set;
		}

		[ForeignKey("Customer")]
		public int CustomerId {
			get;
			set;
		}

		public virtual Customer Customer {
			get;
			set;
		}

		[ForeignKey("Property")]
		public int PropertyId {
			get;
			set;
		}

		public virtual CustomProperty Property {
			get;
			set;
		}

		public string Value {
			get;
			set;
		}
	}
}

