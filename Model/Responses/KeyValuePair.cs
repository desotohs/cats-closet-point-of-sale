using System;

namespace CatsCloset.Model.Responses {
	public class KeyValuePair {
		public string name;
		public string value;

		public KeyValuePair(string name, string value) {
			this.name = name;
			this.value = value;
		}
	}
}

