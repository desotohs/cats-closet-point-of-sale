using System;

namespace CatsCloset.Apis {
	public class SessionSendEvent {
		public bool DefaultPrevented;
		public string SessionId;
		public string Data;

		public void PreventDefault() {
			DefaultPrevented = true;
		}

		public SessionSendEvent(string sessionId, string data) {
			DefaultPrevented = false;
			SessionId = sessionId;
			Data = data;
		}
	}
}

