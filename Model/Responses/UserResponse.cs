using System;

namespace CatsCloset.Model.Responses {
	public class UserResponse {
		public int id;
		public string username;
		public bool storeAccess;
		public bool officeAccess;
		public bool settingsAccess;

		public UserResponse(User user) {
			id = user.Id;
			username = user.Username;
			storeAccess = user.StoreAccess;
			officeAccess = user.OfficeAccess;
			settingsAccess = user.SettingsAccess;
		}
	}
}

