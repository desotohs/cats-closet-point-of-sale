using System;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class TestApi : AbstractApi<EmptyRequest, StatusResponse> {
		protected override StatusResponse Handle(EmptyRequest req) {
			return new StatusResponse(true);
		}

		public TestApi() : base("/test") {
		}
	}
}

