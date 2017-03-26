using System;
using System.Linq;
using System.Threading;
using CatsCloset.Emails;
using CatsCloset.Model;

namespace CatsCloset.Main {
	public class PurchaseReporter {
		private Context Context;
		private Thread thread;

		private void Run() {
			while (true) {
				Option option;
				DateTime lastSent;
				DateTime yesterday = DateTime.Now.Date.AddDays(-1);
				lock (Context) {
					option = Context.Options.FirstOrDefault(o => o.Key == "LastReportSent");
					if (option == null) {
						lastSent = yesterday;
						option = new Option();
						option.Key = "LastReportSent";
						option.Value = lastSent.ToShortDateString();
						Context.Options.Add(option);
						Context.SaveChanges();
					} else {
						lastSent = DateTime.Parse(option.Value);
					}
				}
				if (lastSent.CompareTo(yesterday) < 0) {
					lastSent = lastSent.AddDays(1);
					DateTime dayLater = lastSent.AddDays(1);
					lock (Context) {
						if (Context.History.Any(h => h.Time >= lastSent && h.Time <= dayLater)) {
							lastSent.SendDailyReportEmail();
						}
						option.Value = lastSent.ToShortDateString();
						Context.SaveChanges();
					}
				} else {
					DateTime tomorrow = yesterday.AddDays(2);
					try {
						Thread.Sleep(tomorrow.Subtract(DateTime.Now));
					} catch (ThreadInterruptedException) {
						return;
					}
				}
			}
		}

		public void Stop() {
			thread.Interrupt();
		}

		public PurchaseReporter(Context ctx) {
			Context = ctx;
			thread = new Thread(Run);
			thread.Start();
		}
	}
}

