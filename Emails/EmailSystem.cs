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

		public static void SendPurchaseEmail(Customer customer, History purchase) {
			EmailFactory email = new EmailFactory("PurchaseEmail.html");
			email.Replace(Context, customer, purchase);
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
	}
}

