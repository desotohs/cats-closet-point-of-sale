using System;
using System.ComponentModel.DataAnnotations;

namespace CatsCloset.Model {
	public class User {
		[Key]
		public int Id {
			get;
			set;
		}

		public string Username {
			get;
			set;
		}

		public string Password {
			get;
			set;
		}

		public string Hash {
			get;
			set;
		}

		public bool StoreAccess {
			get;
			set;
		}

		public bool OfficeAccess {
			get;
			set;
		}

		public bool SettingsAccess {
			get;
			set;
		}
	}
}

