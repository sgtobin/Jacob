using System.Net.Mail;

namespace WorkorderGenerator
{
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
			foreach (var workorder in _dbContext.WorkordersToBeSent())
			{
				var pdf = _pdfWriter.Generate(workorder, true);
				workorder.Pdf = pdf.ToArray();

				var mail = new MailMessage
				{
					From = new MailAddress("workorders@jacob.com"),
					Subject = "Workorder"
				};

				//change to appropriate emails
				mail.To.Add(new MailAddress("sean.g.tobin@intel.com"));
				mail.CC.Add(new MailAddress("sean.g.tobin@intel.com"));
				mail.Bcc.Add(new MailAddress("sean.g.tobin@intel.com"));

				mail.Attachments.Add(new Attachment(pdf, "workorder.pdf"));

				new SmtpClient("smtp.intel.com").Send(mail);

				_dbContext.SaveChanges();
			}
		}

	}
}