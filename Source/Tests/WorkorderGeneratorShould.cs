using Microsoft.VisualStudio.TestTools.UnitTesting;
using WorkorderGenerator;

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
		}

		[TestMethod]
		public void EmailWorkOrders()
		{
			var wog = new Generator(new ApplicationDatabaseContext());
			wog.Execute();
		}

		public const string InitDataSql = @"

SET ANSI_NULLS ON
SET QUOTED_IDENTIFIER ON
------------------------------------------------------------------------------------------------

DROP TABLE IF EXISTS dbo.Workorders
DROP TABLE IF EXISTS dbo.Vendors
DROP TABLE IF EXISTS dbo.Clients

CREATE TABLE [dbo].[Clients](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Address1] [nvarchar](150) NOT NULL,
	[Logo] [varbinary](max) NULL,
 CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

INSERT INTO Clients VALUES 
(NEWID(),'Client 1','Client 1 address',null),
(NEWID(),'Client 2','Client 2 address',null)
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

CREATE TABLE [dbo].[Workorders](
	[Id] [uniqueidentifier] NOT NULL,
	[Client_Id] [uniqueidentifier] NOT NULL,
	[Vendor_Id] [uniqueidentifier] NOT NULL,
	[Sow] [varchar](5000) NOT NULL,
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

DECLARE @Client1Id uniqueidentifier;
DECLARE @Client2Id uniqueidentifier;
DECLARE @Vendor1Id uniqueidentifier;
DECLARE @Vendor2Id uniqueidentifier;

SELECT @Client1Id = Id FROM Clients WHERE Name = 'Client 1'
SELECT @Client2Id = Id FROM Clients WHERE Name = 'Client 2'
SELECT @Vendor1Id = Id FROM Vendors WHERE Name = 'Vendor 1'
SELECT @Vendor2Id = Id FROM Vendors WHERE Name = 'Vendor 2'

INSERT INTO Workorders VALUES 
(NEWID(), @Client1Id, @Vendor1Id, 'Do stuff for vendor 1', null),
(NEWID(), @Client2Id, @Vendor2Id, 'Do stuff for vendor 2', null)
";
	}
}
