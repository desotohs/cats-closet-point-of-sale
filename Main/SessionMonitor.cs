using System;
using System.Linq;
using System.Timers;
using CatsCloset.Model;

namespace CatsCloset.Main {
	public class SessionMonitor {
		private const int PurgePeriod = 60 * 60 * 1000;
		private readonly Timer Timer;
		public readonly Context Context;

		public void PurgeSessions() {
			lock ( Context ) {
				Context.SessionMessages
					.RemoveRange(
						Context.SessionMessages
						.Where(
							m => m.LastUpdate + new TimeSpan(1, 0, 0) < DateTime.Now));
			}
		}

		public void Start() {
			Timer.Start();
		}

		public SessionMonitor(Context ctx) {
			Context = ctx;
			Timer = new Timer(PurgePeriod);
			Timer.AutoReset = true;
			Timer.Elapsed += (sender, e) => PurgeSessions();
		}
	}
}

