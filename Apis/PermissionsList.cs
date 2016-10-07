using System;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class PermissionsList : AbstractApi<EmptyRequest, PermissionResponse> {
		protected override PermissionResponse Handle(EmptyRequest req) {
			return new PermissionResponse(RequireAuthentication());
		}

		public PermissionsList() : base("/permissions") {
		}
	}
}

