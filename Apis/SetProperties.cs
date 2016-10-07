using System;
using System.Collections.Generic;
using System.Linq;
using CatsCloset.Model;
using CatsCloset.Model.Responses;

namespace CatsCloset.Apis {
	public class SetProperties : AbstractApi<IEnumerable<string>, StatusResponse> {
		protected override StatusResponse Handle(IEnumerable<string> req) {
			AccessRequire(RequireAuthentication().SettingsAccess);
			foreach ( CustomProperty prop in Context.CustomProperties.Where(p => !req.Contains(p.Name)) ) {
				Context.CustomerProperties.RemoveRange(Context.CustomerProperties.Where(p => p.PropertyId == prop.Id));
				Context.CustomProperties.Remove(prop);
			}
			foreach ( string newProp in req.Except(Context.CustomProperties.Select(p => p.Name)) ) {
				CustomProperty prop = new CustomProperty();
				prop.Name = newProp;
				Context.CustomProperties.Add(prop);
				foreach ( Customer customer in Context.Customers ) {
					CustomerProperty p = new CustomerProperty();
					p.Customer = customer;
					p.Property = prop;
					p.Value = "";
					customer.Properties.Add(p);
				}
			}
			Context.SaveChanges();
			return new StatusResponse(true);
		}

		public SetProperties() : base("/properties/save") {
		}
	}
}

