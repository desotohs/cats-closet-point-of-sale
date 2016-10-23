using System;
using System.Collections.Generic;
using System.Linq;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class UserList : AbstractApi<EmptyRequest, IEnumerable<UserResponse>> {
		protected override IEnumerable<UserResponse> Handle(EmptyRequest req) {
			AccessRequire(RequireAuthentication().SettingsAccess);
			lock ( Context ) {
				return Context.Users
					.ToArray()
					.Select(
						u => new UserResponse(u));
			}
		}

		public UserList() : base("/users") {
		}
	}
}

