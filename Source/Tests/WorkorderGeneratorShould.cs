using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkorderGenerator;
using WorkorderGenerator.Models;

namespace Tests
{
	[TestClass]
	public class WorkorderGeneratorShould
	{
		[AssemblyInitialize]
		public static void SetUpTestData(TestContext testContext)
		{
			var db = new ApplicationDatabaseContext();
			db.Database.ExecuteSqlCommand(InitDataSql);

			var logo = ExtractResource("WorkorderGenerator.Logo.PNG");
			var adc = new ApplicationDatabaseContext();
			foreach (var client in adc.AllClients())
			{
				client.Logo = logo;
			}
			adc.SaveChanges();
		}

		[TestMethod]
		public void EmailWorkOrders()
		{
			var wog = new Generator(new ApplicationDatabaseContext());
			wog.Execute();
		}

		[TestMethod]
		public void GeneratePdf()
		{
			var adc = new ApplicationDatabaseContext();
			var wo = adc.WorkordersToBeSent().First();
			var tempFileName = $"{Path.GetFileNameWithoutExtension(Path.GetTempFileName())}.pdf";
			File.WriteAllBytes(tempFileName, new PdfWriter().Generate(wo, true).ToArray());
			Process.Start(tempFileName);
		}

		public static byte[] ExtractResource(string filename)
		{
			using (var resFilestream = typeof(Workorder).Assembly.GetManifestResourceStream(filename))
			{
				if (resFilestream == null) return null;
				var ba = new byte[resFilestream.Length];
				resFilestream.Read(ba, 0, ba.Length);
				return ba;
			}
		}

		public const string InitDataSql = @"

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
------------------------------------------------------------------------------------------------

DROP TABLE IF EXISTS dbo.Workorders
DROP TABLE IF EXISTS dbo.Vendors
DROP TABLE IF EXISTS dbo.Customers
DROP TABLE IF EXISTS dbo.Employees
DROP TABLE IF EXISTS dbo.Clients

CREATE TABLE [dbo].[Clients](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Address1] [nvarchar](150) NOT NULL,
	[Address2] [nvarchar](150) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NOT NULL,
	[Zip1] [nvarchar](50) NOT NULL,
	[Zip2] [nvarchar](50) NOT NULL,
	[Telephone] [nvarchar](100) NOT NULL,
	[Fax] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[WebSite] [nvarchar](100) NOT NULL,
	[Logo] [varbinary](max) NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

INSERT INTO Clients VALUES 
(NEWID(),'Client 1','address line 1','address line 2','city','state','zip1','zip2','telephone','fax','someone@somewhere.com','https://website.com', null),
(NEWID(),'Client 2','address line 1','address line 2','city','state','zip1','zip2','telephone','fax','someone@somewhere.com','https://website.com', null)
------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[Vendors](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_Vendors] PRIMARY KEY CLUSTERED 
(
	[Id] ASC 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO Vendors VALUES 
(NEWID(),'Vendor 1'),
(NEWID(),'Vendor 2')
------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[Customers](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[StoreName] [nvarchar](150) NOT NULL,
	[Address1] [nvarchar](150) NOT NULL,
	[Address2] [nvarchar](150) NOT NULL,
	[City] [nvarchar](100) NOT NULL,
	[State] [nvarchar](100) NOT NULL,
	[Zip1] [nvarchar](50) NOT NULL,
	[Zip2] [nvarchar](50) NOT NULL,
	[Telephone] [nvarchar](100) NOT NULL,
	[Fax] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NOT NULL,
	[ContactPerson] [nvarchar](150) NOT NULL,
	[SiteInfo] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO Customers VALUES 
(NEWID(),'Customer 1','storename','address line 1','address line 2','city','state','zip1','zip2','telephone','fax','someone@somewhere.com','contact person', 'site info'),
(NEWID(),'Customer 2','storename','address line 1','address line 2','city','state','zip1','zip2','telephone','fax','someone@somewhere.com','contact person', 'site info')
------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[Employees](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Telephone] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC 
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO Employees VALUES 
(NEWID(),'Employee 1','telephone'),
(NEWID(),'Employee 2','telephone')
------------------------------------------------------------------------------------------------

CREATE TABLE [dbo].[Workorders](
	[Id] [uniqueidentifier] NOT NULL,
	[Client_Id] [uniqueidentifier] NOT NULL,
	[Vendor_Id] [uniqueidentifier] NOT NULL,
	[Customer_Id] [uniqueidentifier] NOT NULL,
	[Employee_Id] [uniqueidentifier] NOT NULL,
	[WorkOrderNumber] [nvarchar](100) NOT NULL,
	[WorkOrderDate] [DateTime] NOT NULL,
	[CustomerWorkOrderNumber] [nvarchar](100) NOT NULL,
	[Sow] [nvarchar](1000) NOT NULL,
	[SuggestedBeginDatetime] [DateTime] NOT NULL,
	[SuggestedEndDatetime] [DateTime] NOT NULL,
	[IvrNumber] [nvarchar](1000) NOT NULL,
	[IvrCode] [nvarchar](1000) NOT NULL,
	[NTE] [decimal] NOT NULL,
	[Pdf] [varbinary](max) NULL,
 CONSTRAINT [PK_Workorders] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

ALTER TABLE [dbo].[Workorders]  WITH CHECK ADD  CONSTRAINT [FK_Workorders_Clients] FOREIGN KEY([Client_Id])
REFERENCES [dbo].[Clients] ([Id])
ALTER TABLE [dbo].[Workorders] CHECK CONSTRAINT [FK_Workorders_Clients]

ALTER TABLE [dbo].[Workorders]  WITH CHECK ADD  CONSTRAINT [FK_Workorders_Vendors] FOREIGN KEY([Vendor_Id])
REFERENCES [dbo].[Vendors] ([Id])
ALTER TABLE [dbo].[Workorders] CHECK CONSTRAINT [FK_Workorders_Vendors]

ALTER TABLE [dbo].[Workorders]  WITH CHECK ADD  CONSTRAINT [FK_Workorders_Customers] FOREIGN KEY([Customer_Id])
REFERENCES [dbo].[Customers] ([Id])
ALTER TABLE [dbo].[Workorders] CHECK CONSTRAINT [FK_Workorders_Customers]

ALTER TABLE [dbo].[Workorders]  WITH CHECK ADD  CONSTRAINT [FK_Workorders_Employees] FOREIGN KEY([Employee_Id])
REFERENCES [dbo].[Employees] ([Id])
ALTER TABLE [dbo].[Workorders] CHECK CONSTRAINT [FK_Workorders_Employees]

DECLARE @Client1Id uniqueidentifier;
DECLARE @Client2Id uniqueidentifier;
DECLARE @Vendor1Id uniqueidentifier;
DECLARE @Vendor2Id uniqueidentifier;
DECLARE @Customer1Id uniqueidentifier;
DECLARE @Customer2Id uniqueidentifier;
DECLARE @Employee1Id uniqueidentifier;
DECLARE @Employee2Id uniqueidentifier;

SELECT @Client1Id = Id FROM Clients WHERE Name = 'Client 1'
SELECT @Client2Id = Id FROM Clients WHERE Name = 'Client 2'
SELECT @Vendor1Id = Id FROM Vendors WHERE Name = 'Vendor 1'
SELECT @Vendor2Id = Id FROM Vendors WHERE Name = 'Vendor 2'
SELECT @Customer1Id = Id FROM Customers WHERE Name = 'Customer 1'
SELECT @Customer2Id = Id FROM Customers WHERE Name = 'Customer 2'
SELECT @Employee1Id = Id FROM Employees WHERE Name = 'Employee 1'
SELECT @Employee2Id = Id FROM Employees WHERE Name = 'Employee 2'

INSERT INTO Workorders VALUES 
(NEWID(), @Client1Id, @Vendor1Id, @Customer1Id, @Employee1Id, 'wo#', getdate(), 'customer wo#', 'Do stuff for vendor 1', getdate(), getdate(), 'ivr#', 'ivr code', 1.5, null),
(NEWID(), @Client2Id, @Vendor2Id, @Customer2Id, @Employee2Id, 'wo#', getdate(), 'customer wo#', 'Do stuff for vendor 2', getdate(), getdate(), 'ivr#', 'ivr code', 1.5, null)
";
	}
}
