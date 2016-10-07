using System;
using System.Collections.Generic;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Requests;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SetCustomer : AbstractApi<SaveCustomerRequest, StatusResponse> {
		protected override StatusResponse Handle(SaveCustomerRequest req) {
			AccessRequire(RequireAuthentication().SettingsAccess);
			Customer customer = Context.Customers
				.FirstOrDefault(
					c => c.Name == req.name);
			if ( customer == null ) {
				customer = new Customer();
				customer.Name = req.name;
				if ( customer.Properties == null ) {
					customer.Properties = new List<CustomerProperty>();
				}
				Context.Customers.Add(customer);
			}
			customer.Barcode = req.code;
			customer.Balance = req.balance;
			foreach ( CustomerProperty prop in customer.Properties.ToArray() ) {
				KeyValuePair newProp = req.properties
					.FirstOrDefault(
						p => p.name == prop.Property.Name);
				if ( newProp == null ) {
					customer.Properties.Remove(prop);
					Context.CustomerProperties.Remove(prop);
				} else {
					prop.Value = newProp.value;
					req.properties.Remove(newProp);
				}
			}
			foreach ( KeyValuePair newProp in req.properties ) {
				CustomerProperty prop = new CustomerProperty();
				prop.CustomerId = customer.Id;
				prop.Customer = customer;
				prop.Property = Context.CustomProperties
					.First(
						p => p.Name == req.name);
				prop.PropertyId = prop.Property.Id;
				prop.Value = newProp.value;
				Context.CustomerProperties.Add(prop);
				customer.Properties.Add(prop);
			}
			Context.SaveChanges();
			return new StatusResponse(true);
		}

		public SetCustomer() : base("/customer/save") {
		}
	}
}

