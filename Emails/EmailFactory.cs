using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml;
using CatsCloset.Model;

namespace CatsCloset.Emails {
	public class EmailFactory {
		public readonly XmlDocument EmailDocument;

		private string FormatCurrency(double value) {
			return string.Format("${0:N2}", value);
		}

		private object GetReplacement(string name, Context ctx, Customer customer, History currentPurchase, HistoryPurchase purchase, History recentPurchase) {
			switch ( name ) {
			case "Options.StoreName":
				return ctx.Options
					.First(
						o => o.Key == "StoreName")
					.Value;
			case "Customer.Balance":
				return FormatCurrency(customer.Balance);
			case "Purchase.Name":
				return purchase.Product.Name;
			case "Purchase.Quantity":
				return purchase.Amount;
			case "Purchase.UnitPrice":
				return FormatCurrency(purchase.Product.Price);
			case "Purchases.Total":
				return FormatCurrency(Math.Abs(currentPurchase.BalanceChange));
			case "RecentPurchase.Date":
				return recentPurchase.Time;
			case "RecentPurchase.Amount":
				return FormatCurrency(-recentPurchase.BalanceChange);
			case "Options.AdminEmail":
				return ctx.Options
					.First(
						o => o.Key == "AdminEmail")
					.Value;
			case "Options.MailingAddress":
				return ctx.Options
					.First(
						o => o.Key == "MailingAddress")
					.Value;
			default:
				throw new ArgumentException("Unknown replacement token");
			}
		}

		private void Replace(Context ctx, XmlElement element, Customer customer, History currentPurchase, HistoryPurchase purchase, History recentPurchase) {
			foreach ( XmlElement child in element.ChildNodes
				.Cast<XmlNode>()
				.Where(
					n => n is XmlElement)
				.Cast<XmlElement>()
				.Where(
					n => (n.Name == "span" ||
					n.Name == "a") &&
					n.Attributes
					.Cast<XmlAttribute>()
					.Any(
						a => a.Name == "class" &&
						a.Value == "replacement")) ) {
				// Each of these is a replacement span
				child.RemoveAttribute("class");
				child.InnerText = GetReplacement(child.InnerText, ctx, customer, currentPurchase, purchase, recentPurchase).ToString();
				if ( child.Name == "a" ) {
					child.SetAttribute("href", string.Concat(child.InnerText.Contains("@") ? "mailto:" : "", child.InnerText));
				}
			}
			foreach ( XmlElement child in element.ChildNodes
				.Cast<XmlNode>()
				.Where(
					n => n is XmlElement)
				.Cast<XmlElement>()
				.ToArray() ) {
				if ( child.Name == "span" && child.Attributes
					.Cast<XmlAttribute>()
					.Any(
					     a => a.Name == "class" &&
					     a.Value == "repeat") ) {
					IEnumerable<HistoryPurchase> purchases;
					IEnumerable<History> recentPurchases;
					switch ( child.GetAttribute("data-repeat") ) {
					case "Purchases":
						purchases = currentPurchase.Purchases;
						recentPurchases = Enumerable.Repeat(recentPurchase, purchases.Count());
						break;
					case "RecentPurchases":
						purchases = Enumerable.Repeat(purchase, 5);
						recentPurchases = customer.PurchaseHistory.Where(h => h.BalanceChange < 0).OrderByDescending(h => h.Time).Take(5);
						break;
					default:
						throw new ArgumentException("Unknown repeat token");
					}
					using ( IEnumerator<HistoryPurchase> purchaseEnum = purchases.GetEnumerator() ) {
						using ( IEnumerator<History> recentPurchaseEnum = recentPurchases.GetEnumerator() ) {
							while ( purchaseEnum.MoveNext() && recentPurchaseEnum.MoveNext() ) {
								XmlElement clone = (XmlElement) child.Clone();
								element.AppendChild(clone);
								Replace(ctx, clone, customer, currentPurchase, purchaseEnum.Current, recentPurchaseEnum.Current);
							}
						}
					}
					element.RemoveChild(child);
				} else {
					Replace(ctx, child, customer, currentPurchase, purchase, recentPurchase);
				}
			}
		}

		public void Replace(Context ctx, Customer customer, History currentPurchase) {
			Replace(ctx, EmailDocument.DocumentElement, customer, currentPurchase, null, null);
		}

		public override string ToString() {
			StringBuilder builder = new StringBuilder();
			using ( XmlWriter writer = XmlWriter.Create(builder) ) {
				EmailDocument.WriteTo(writer);
			}
			return builder.ToString();
		}

		public EmailFactory(XmlDocument doc) {
			EmailDocument = doc;
		}

		private static XmlDocument ReadDocument(Stream stream, bool leaveOpen) {
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);
			if ( !leaveOpen ) {
				stream.Close();
				stream.Dispose();
			}
			return doc;
		}

		public EmailFactory(Stream stream, bool leaveOpen) : this(ReadDocument(stream, leaveOpen)) {
		}

		public EmailFactory(Stream stream) : this(stream, true) {
		}

		public EmailFactory(string filename) : this(new FileStream(filename, FileMode.Open), false) {
		}
	}
}

