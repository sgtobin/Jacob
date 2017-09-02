using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WorkorderGenerator.Models;

namespace WorkorderGenerator
{
    public class ApplicationDatabaseContext : DbContext
    {
	    private readonly IQueryable<Workorder> _workorders;
	    private readonly IQueryable<Client> _clients;

		public ApplicationDatabaseContext() : base("mydb")
		{
			var workorders = Set<Workorder>();
			_workorders = workorders
				.Include(wo => wo.Client)
				.Include(wo => wo.Vendor)
				.Include(wo => wo.Customer)
				.Include(wo => wo.Employee);

			var clients = Set<Client>();
			_clients = clients;
		}

	    public IEnumerable<Workorder> WorkordersToBeSent()
	    {
		    var wos = _workorders; // add where clauses etc here - e.g: _workorders.Where(wo => wo.Pdf == null)
			foreach (var workorder in wos)
		    {
			    workorder.Initialize();
		    }
		    return wos;
	    }

	    public IEnumerable<Client> AllClients()
	    {
		    var clients = _clients;
		    foreach (var client in clients)
		    {
			    client.Initialize();
		    }
			return clients;
	    }

	    protected override void OnModelCreating(DbModelBuilder modelBuilder)
	    {
			modelBuilder.Entity<Workorder>()
				.ToTable("Workorders");

		    modelBuilder.Entity<Client>()
			    .Ignore(c => c.Address);

		    modelBuilder.Entity<Customer>()
			    .Ignore(c => c.Address);
	    }
	}
}
