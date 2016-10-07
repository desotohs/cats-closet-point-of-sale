using System;

namespace CatsCloset.Model.Responses {
	public class PermissionResponse {
		public bool store;
		public bool office;
		public bool settings;

		public PermissionResponse(User user) {
			store = user.StoreAccess;
			office = user.OfficeAccess;
			settings = user.SettingsAccess;
		}
	}
}

