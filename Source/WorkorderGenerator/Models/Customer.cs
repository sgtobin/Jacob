using System;

namespace WorkorderGenerator.Models
{
	public class Customer
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string StoreName { get; set; }
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip1 { get; set; }
		public string Zip2 { get; set; }
		public string Telephone { get; set; }
		public string Fax { get; set; }
		public string Email { get; set; }
		public string ContactPerson { get; set; }
		public string SiteInfo { get; set; }
		public Address Address { get; private set; }

		public void Initialize()
		{
			Address = new Address(Address1, Address2, City, State, Zip1, Zip2);
		}
	}
}