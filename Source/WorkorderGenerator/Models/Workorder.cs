using System;

namespace WorkorderGenerator.Models
{
	public class Workorder
	{
		public Guid Id { get; set; }
		public string WorkOrderNumber { get; set; }
		public DateTime WorkOrderDate { get; set; }
		public string CustomerWorkOrderNumber { get; set; }
		public Client Client { get; set; }
		public Employee Employee { get; set; }
		public Vendor Vendor { get; set; }
		public Customer Customer { get; set; }
		public string Sow { get; set; }
		public DateTime SuggestedBeginDatetime { get; set; }
		public DateTime SuggestedEndDatetime { get; set; }
		public string IvrNumber { get; set; }
		public string IvrCode { get; set; }
		public decimal Nte { get; set; }
		public byte[] Pdf { get; set; }

		public void Initialize()
		{
			Customer.Initialize();
			Client.Initialize();
			Vendor.Initialize();
			Employee.Initialize();
		}
	}
}