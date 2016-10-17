using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using CatsCloset.Model;

namespace CatsCloset.Emails {
	public class EmailConnection {
		private readonly SmtpClient Client;
		private readonly MailAddress FromAddress;
		private readonly string StoreName;

		public void SendEmail(string content, string to) {
			MailMessage message = new MailMessage();
			message.Body = content;
			message.From = FromAddress;
			message.IsBodyHtml = true;
			message.Subject = string.Concat(StoreName, " Purchase Receipt");
			message.To.Add(to);
			Client.Send(message);
		}

		private string GetOptionString(Context ctx, string key) {
			Option opt = ctx.Options
				.FirstOrDefault(
					o => o.Key == key);
			if ( opt == null ) {
				throw new InvalidOperationException(key);
			}
			return opt.Value;
		}

		public EmailConnection(Context ctx) {
			FromAddress = new MailAddress(GetOptionString(ctx, "EmailFrom"));
			StoreName = GetOptionString(ctx, "StoreName");
			Client = new SmtpClient(GetOptionString(ctx, "SmtpServer"), int.Parse(GetOptionString(ctx, "SmtpPort")));
			Client.EnableSsl = bool.Parse(GetOptionString(ctx, "SmtpSsl"));
			Client.Credentials = new NetworkCredential(GetOptionString(ctx, "SmtpUser"), GetOptionString(ctx, "SmtpPass"));
			ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
		}
	}
}

