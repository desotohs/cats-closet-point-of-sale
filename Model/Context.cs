using System;
using System.Data.Entity;

namespace CatsCloset.Model {
	public class Context : DbContext {
		public virtual DbSet<Customer> Customers {
			get;
			set;
		}

		public virtual DbSet<CustomerProperty> CustomerProperties {
			get;
			set;
		}

		public virtual DbSet<CustomProperty> CustomProperties {
			get;
			set;
		}

		public virtual DbSet<Image> Images {
			get;
			set;
		}

		public virtual DbSet<Option> Options {
			get;
			set;
		}

		public virtual DbSet<Product> Products {
			get;
			set;
		}

		public virtual DbSet<User> Users {
			get;
			set;
		}

		public virtual DbSet<SessionMessage> SessionMessages {
			get;
			set;
		}

		public virtual DbSet<History> History {
			get;
			set;
		}

		public virtual DbSet<HistoryPurchase> HistoryPurchases {
			get;
			set;
		}
	}
}

