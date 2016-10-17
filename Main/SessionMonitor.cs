using System;
using System.Data.Entity;
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
				DateTime hourAgo = DateTime.Now - new TimeSpan(1, 0, 0);
				Context.SessionMessages
					.RemoveRange(
						Context.SessionMessages
						.Where(
							m => m.LastUpdate < hourAgo));
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

