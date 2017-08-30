using System;

namespace WorkorderGenerator
{
	public class Workorder
	{
		public Guid Id { get; set; }
		public Client Client { get; set; }
		public Vendor Vendor { get; set; }
		public string Sow { get; set; }
		public byte[] Pdf { get; set; }
	}
}