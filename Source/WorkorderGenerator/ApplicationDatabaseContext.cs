using System;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using Color = MigraDoc.DocumentObjectModel.Color;

namespace WorkorderGenerator
{
    public class ApplicationDatabaseContext : DbContext
    {
	    public DbSet<Workorder> Workorders { get; set; }

		public ApplicationDatabaseContext() : base("mydb")
	    {
	    }

	    protected override void OnModelCreating(DbModelBuilder modelBuilder)
	    {
		    
	    }
	}

	public class Generator
	{
		private readonly ApplicationDatabaseContext _dbContext;
		private readonly PdfWriter _pdfWriter;

		public Generator(ApplicationDatabaseContext dbContext)
		{
			_dbContext = dbContext;
			_pdfWriter = new PdfWriter();
		}

		public void Execute()
		{
			var wos = _dbContext.Workorders
				.Include(wo => wo.Client)
				.Include(wo => wo.Vendor)
				.ToList();

			foreach (var workorder in wos)
			{
				var pdf = _pdfWriter.Generate(workorder);
				workorder.Pdf = pdf.ToArray();

				var mail = new MailMessage
				{
					From = new MailAddress("workorders@jacob.com"),
					Subject = "Workorder"
				};
				mail.To.Add(new MailAddress("sean.g.tobin@intel.com"));
				mail.Attachments.Add(new Attachment(pdf, "workorder.pdf"));

				new SmtpClient("smtp.intel.com").Send(mail);

				_dbContext.SaveChanges();
			}
		}

	}

	public class PdfWriter
	{
		private static Document _document;
		private Section _section;

		public MemoryStream Generate(Workorder workorder)
		{
			InitializeDocument();

			_section.AddParagraph($"{workorder.Client.Name}", "TitleStyle");

			_section.AddPageBreak();

			_section.AddParagraph($"{workorder.Sow}", "Normal");

			return AsStream();
		}

		private void InitializeDocument()
		{
			_document = new Document
			{
				Info = { Author = "Jacob Inc", Comment = "Jacob Inc", Subject = "Workorder" }
			};

			var style = _document.Styles["Normal"];
			style.Font.Name = "Verdana";
			style.Font.Size = 12;

			style = _document.Styles.AddStyle("TitleStyle", "Normal");
			style.Font.Color = GetColor("#0055A5");
			style.Font.Size = 16;
			style.ParagraphFormat.SpaceBefore = new Unit(0, UnitType.Centimeter);
			style.ParagraphFormat.SpaceAfter = new Unit(.125, UnitType.Centimeter);

			_section = _document.AddSection();
		}

		protected Color GetColor(string color)
		{
			var convertFromString = new ColorConverter().ConvertFromString(color);
			if (convertFromString == null)
				throw new Exception("Cant identify color");
			var cc = (System.Drawing.Color)convertFromString;
			return new Color(cc.R, cc.G, cc.B);
		}

		private MemoryStream AsStream()
		{
			var pdfRenderer = new PdfDocumentRenderer(false, PdfFontEmbedding.Always)
			{
				Document = _document
			};

			pdfRenderer.RenderDocument();

			var stream = new MemoryStream();
			pdfRenderer.Save(stream, false);
			return stream;
		}

	}
}
