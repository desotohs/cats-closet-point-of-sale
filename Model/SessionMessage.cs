using System;
using System.ComponentModel.DataAnnotations;

namespace CatsCloset.Model {
	public class SessionMessage {
		[Key]
		public string Id {
			get;
			set;
		}

		public string Content {
			get;
			set;
		}

		public DateTime LastUpdate {
			get;
			set;
		}
	}
}

