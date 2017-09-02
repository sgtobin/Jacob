using System.Collections.Generic;
using System.Linq;

namespace WorkorderGenerator.Models
{
	public class Address
	{
		public string Address1 { get; set; }
		public string Address2 { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Zip1 { get; set; }
		public string Zip2 { get; set; }

		public string AddressLines => AddressLine(", ", Address1, Address2);

		public string CityAndState => AddressLine(", ", City, State);
		public string FullZip => AddressLine("-", Zip1, Zip2);
		public string CityAndStateAndZip => AddressLine(", ", CityAndState, FullZip);
		public string Full => AddressLine(", ",AddressLines, CityAndStateAndZip);

		public Address(string address1, string address2, string city, string state, string zip1, string zip2)
		{
			Address1 = address1;
			Address2 = address2;
			City = city;
			State = state;
			Zip1 = zip1;
			Zip2 = zip2;
		}

		private string AddressLine(string deliminator, params string[] parts)
		{
			var bits = parts.Where(part => !string.IsNullOrWhiteSpace(part)).ToList();
			return string.Join(deliminator, bits);
		}
	}
}