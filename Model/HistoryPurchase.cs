using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatsCloset.Model {
	public class HistoryPurchase {
		[Key]
		public int Id {
			get;
			set;
		}

		[ForeignKey("Product")]
		public int ProductId {
			get;
			set;
		}

		public virtual Product Product {
			get;
			set;
		}

		[ForeignKey("History")]
		public int HistoryId {
			get;
			set;
		}

		public virtual History History {
			get;
			set;
		}

		public int Amount {
			get;
			set;
		}
	}
}

