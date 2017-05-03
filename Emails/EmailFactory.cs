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

		private string FormatCurrency(int value) {
			return string.Format("${0:N2}", value / 100.0);
		}

		private object GetReplacement(string name, Context ctx, Customer customer, History currentPurchase, HistoryPurchase purchase, History recentPurchase, DateTime reportDate, Product product) {
			switch ( name ) {
			case "Customer.Balance":
				return FormatCurrency(customer.Balance);
			case "Customer.Pin":
				return customer.Pin;
			case "Customer.Name":
				return customer.Name;
			case "Product.Inventory":
				return product.InventoryAmount;
			case "Product.Name":
				return product.Name;
			case "Product.Price":
				return FormatCurrency(product.Price);
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
			case "Options.StoreName":
				return ctx.Options
					.First(
						o => o.Key == "StoreName")
					.Value;
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
			case "Report.Date":
				return reportDate.ToShortDateString();
			default:
				throw new ArgumentException(string.Format("Unknown replacement token '{0}'", name));
			}
		}

		private void Replace(Context ctx, XmlElement element, Customer customer, History currentPurchase, HistoryPurchase purchase, History recentPurchase, DateTime reportDate, Product product) {
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
				child.InnerText = GetReplacement(child.InnerText, ctx, customer, currentPurchase, purchase, recentPurchase, reportDate, product).ToString();
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
					IEnumerable<Customer> customers;
					IEnumerable<History> currentPurchases;
					IEnumerable<HistoryPurchase> purchases;
					IEnumerable<History> recentPurchases;
					IEnumerable<Product> products;
					switch ( child.GetAttribute("data-repeat") ) {
					case "Purchases":
						purchases = currentPurchase.BalanceChange > 0 ? new HistoryPurchase[] {
							new HistoryPurchase() {
								Product = new Product() {
									Name = "Deposit",
									Price = currentPurchase.BalanceChange
								},
								Amount = 1
							}
						} : currentPurchase.Purchases.ToArray();
						customers = Enumerable.Repeat(customer, purchases.Count());
						currentPurchases = Enumerable.Repeat(currentPurchase, purchases.Count());
						recentPurchases = Enumerable.Repeat(recentPurchase, purchases.Count());
						products = Enumerable.Repeat(product, purchases.Count());
						break;
					case "RecentPurchases":
						customers = Enumerable.Repeat(customer, 5);
						currentPurchases = Enumerable.Repeat(currentPurchase, 5);
						purchases = Enumerable.Repeat(purchase, 5);
						recentPurchases = customer.PurchaseHistory.Where(h => h.BalanceChange < 0).OrderByDescending(h => h.Time).Take(5);
						products = Enumerable.Repeat(product, 5);
						break;
					case "DailyPurchases":
						DateTime lowerBound = reportDate.Date;
						DateTime upperBound = reportDate.Date.AddDays(1);
						currentPurchases = recentPurchases = ctx.History.Where(h => h.Time >= lowerBound && h.Time <= upperBound).ToArray();
						customers = recentPurchases.Select(h => h.Customer).ToArray();
						purchases = Enumerable.Repeat(purchase, recentPurchases.Count()).ToArray();
						products = Enumerable.Repeat(product, recentPurchases.Count());
						break;
					case "Products":
						int c = ctx.Products.Count();
						customers = Enumerable.Repeat(customer, c);
						currentPurchases = Enumerable.Repeat(currentPurchase, c);
						purchases = Enumerable.Repeat(purchase, c);
						recentPurchases = Enumerable.Repeat(recentPurchase, c);
						products = ctx.Products;
						break;
					default:
						throw new ArgumentException("Unknown repeat token");
					}
					using (IEnumerator<Customer> customerEnum = customers.GetEnumerator()) {
						using (IEnumerator<History> currentPurchaseEnum = currentPurchases.GetEnumerator()) {
							using (IEnumerator<HistoryPurchase> purchaseEnum = purchases.GetEnumerator()) {
								using (IEnumerator<History> recentPurchaseEnum = recentPurchases.GetEnumerator()) {
									using (IEnumerator<Product> productEnum = products.GetEnumerator()) {
										while (customerEnum.MoveNext() && currentPurchaseEnum.MoveNext() && purchaseEnum.MoveNext() && recentPurchaseEnum.MoveNext() && productEnum.MoveNext()) {
											XmlElement clone = (XmlElement) child.Clone();
											element.AppendChild(clone);
											Replace(ctx, clone, customerEnum.Current, currentPurchaseEnum.Current, purchaseEnum.Current, recentPurchaseEnum.Current, reportDate, productEnum.Current);
										}
									}
								}
							}
						}
					}
					element.RemoveChild(child);
				} else {
					Replace(ctx, child, customer, currentPurchase, purchase, recentPurchase, reportDate, product);
				}
			}
		}

		public void Replace(Context ctx, Customer customer, History currentPurchase, DateTime reportDate) {
			Replace(ctx, EmailDocument.DocumentElement, customer, currentPurchase, null, null, reportDate, null);
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

