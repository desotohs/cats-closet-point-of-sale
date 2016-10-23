using System;

namespace CatsCloset.Model.Requests {
	public class SaveUserRequest {
		public int id;
		public string username;
		public string password;
		public bool storeAccess;
		public bool officeAccess;
		public bool settingsAccess;
		public bool invalidateToken;
	}
}

