using System;
using System.Linq;
using CatsCloset.Model;

namespace CatsCloset.Emails {
	public static class EmailSystem {
		private static Context Context;
		private static EmailConnection Connection;

		private static void Send(string message, string to) {
			if ( Connection != null ) {
				Connection.SendEmail(message, to);
			}
		}

		public static void Init(Context context) {
			Context = context;
			try {
				Connection = new EmailConnection(context);
			} catch ( InvalidOperationException ex ) {
				Console.Error.WriteLine("Could not create email client.  Please set the {0} option in the database.", ex.Message);
				Connection = null;
			}
		}

		public static void SendEmail(string name, Customer customer, History history) {
			EmailFactory email = new EmailFactory(name);
			email.Replace(Context, customer, history);
			string addr = customer.Properties
				.First(
					p => p.Property.Name ==
					Context.Options
					.First(
						o => o.Key == "EmailProperty")
					.Value)
				.Value;
			Send(email.ToString(), addr);
		}

		public static void SendPurchaseEmail(this Customer customer, History purchase) {
			SendEmail("PurchaseEmail.html", customer, purchase);
		}

		public static void SendDepositEmail(this Customer customer, History deposit) {
			SendEmail("DepositEmail.html", customer, deposit);
		}

		public static void SendSetPinEmail(this Customer customer) {
			SendEmail("SetPinEmail.html", customer, null);
		}
	}
}

