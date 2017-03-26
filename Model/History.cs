using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatsCloset.Model {
	public class History {
		[Key]
		public int Id {
			get;
			set;
		}

		public DateTime Time {
			get;
			set;
		}

		[ForeignKey("User")]
		public int UserId {
			get;
			set;
		}

		public virtual User User {
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

		public int BalanceChange {
			get;
			set;
		}

		public virtual List<HistoryPurchase> Purchases {
			get;
			set;
		}
	}
}

