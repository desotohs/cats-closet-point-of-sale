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

		public byte[] PasswordHash {
			get;
			set;
		}

		public byte[] Salt {
			get;
			set;
		}

		public string Token {
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

