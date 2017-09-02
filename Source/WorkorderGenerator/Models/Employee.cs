using System;

namespace WorkorderGenerator.Models
{
	public class Employee
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Telephone { get; set; }

		public void Initialize()
		{
		}
	}
}