using System;
using System.Drawing;
using System.IO;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using WorkorderGenerator.Models;
using Color = MigraDoc.DocumentObjectModel.Color;

namespace WorkorderGenerator
{
	public static class PdfExtensions
	{
		public static Cell Write(this Cell cell)
		{
			cell.AddParagraph();
			return cell;
		}
		public static Cell Write(this Cell cell, string text, ParagraphFormat format = null)
		{
			var paragraph = cell.AddParagraph(text);
			if (format != null)
				paragraph.Format = format;
			return cell;
		}
		public static Section Write(this Section section, string text, ParagraphFormat format = null)
		{
			var paragraph = section.AddParagraph(text);
			if (format != null)
				paragraph.Format = format;
			return section;
		}
		public static Cell Write(this Cell cell, string label, string text, ParagraphFormat format = null)
		{
			var paragraph = cell.AddParagraph();
			if (format != null)
				paragraph.Format = format;
			paragraph.AddFormattedText(label, TextFormat.Bold);
			paragraph.AddText(text);
			return cell;
		}
		public static Section Write(this Section section, string label, string text, ParagraphFormat format = null)
		{
			var paragraph = section.AddParagraph();
			if (format != null)
				paragraph.Format = format;
			paragraph.AddFormattedText(label, TextFormat.Bold);
			paragraph.AddText(text);
			return section;
		}

		public static Cell Borders(this Cell cell, bool top = false, bool right = false, bool bottom = false, bool left = false)
		{
			cell.Borders.Top.Visible = top;
			cell.Borders.Right.Visible = right;
			cell.Borders.Bottom.Visible = bottom;
			cell.Borders.Left.Visible = left;
			return cell;
		}
	}

	public class PdfWriter
	{
		private static Document _document;
		private Section _section;

		public MemoryStream Generate(Workorder workorder, bool writeAttline)
		{
			InitializeDocument();

			WriteDocumentHeader();

			WritePage1(workorder);

			_section.AddPageBreak();

			WritePage2(workorder, writeAttline);

			return AsStream();
		}

		private void WritePage2(Workorder workorder, bool writeAttline)
		{
			WriteLogoAndAddress(workorder);

			WritePage2Notes();

			WriteCustomerDetails(workorder);

			WriteDateAndTech();

			WriteWorkDescription();

			WriteMaterialsUsed();

			WriteFollowUp();

			WriteCustomerFeedback(workorder);

			if (writeAttline)
			{
				_section.Write($"****FOR ALL AT&T RETAIL STORE WORK********** AT&T UID #: {string.Join(string.Empty, Enumerable.Repeat("_", 10))}");
			}
		}

		private void WriteCustomerFeedback(Workorder workorder)
		{
			_section.AddParagraph();
			_section.AddParagraph();
			_section.AddParagraph();
			_section.AddParagraph();
			_section.Write("CUSTOMER/STORE PERSONNEL SECTION",
				new ParagraphFormat {Alignment = ParagraphAlignment.Center, Font = {Bold = true}});
			_section.Write(
				$"Please call CG Facilities if you have any concerns/questions regarding the work completed on this call. {workorder.Client.Telephone}");
			_section.AddParagraph();
			_section.Write("Tech Performance (Circle One):", $"{Spaces(7)}Excellent{Spaces(7)}Good{Spaces(7)}Fair{Spaces(7)}Poor");
			_section.Write("Comments:", new ParagraphFormat { Font = { Bold = true } });
			Lines(1);
			_section.AddParagraph();
			var table = Table(8, 8);
			var row = table.AddRow();
			row.Cells[0].Write("Printed name: ", $"{string.Join(string.Empty, Enumerable.Repeat("_", 18))}");
			row.Cells[1].Write("Date : ", $"{string.Join(string.Empty, Enumerable.Repeat("_", 18))}");
			_section.AddParagraph();
			table = Table(8, 8);
			row = table.AddRow();
			row.Cells[0].Write($"Signature: ", $"{string.Join(string.Empty, Enumerable.Repeat("_", 18))}");
			row.Cells[1].Write($"Time : ", $"{string.Join(string.Empty, Enumerable.Repeat("_", 18))}");
			_section.AddParagraph();
		}

		private void Lines(int count)
		{
			for (var i = 0; i < count; i++)
			{
				_section.Write(string.Join(string.Empty, Enumerable.Repeat('\u00A0', 126)), new ParagraphFormat { Font = { Underline = Underline.Single } });
			}
		}

		private string Spaces(int count)
		{
			return string.Join(string.Empty, Enumerable.Repeat('\u00A0', count));
		}

		private void WriteFollowUp()
		{
			var table = Table(16);
			var row = table.AddRow();
			row.Cells[0].Write("Is Follow Up Required:",$"{Spaces(7)}Yes{Spaces(7)}No");
			row.Cells[0].Write("Additional Work Needed:", new ParagraphFormat { Font = {Bold = true}});
			Lines(4);
			_section.AddParagraph();
		}

		private void WriteMaterialsUsed()
		{
			var table = Table(16);
			var row = table.AddRow();
			row.Cells[0].Write("Materials Used:", new ParagraphFormat { Font = { Bold = true } });
			Lines(4);
			_section.AddParagraph();
		}

		private void WriteWorkDescription()
		{
			var table = Table(16);
			var row = table.AddRow();
			row.Cells[0].Write("Description of work performed:", new ParagraphFormat { Font = {Bold = true}});
			Lines(4);
			_section.AddParagraph();
		}

		private void WriteDateAndTech()
		{
			var table = Table(8, 8);
			var row = table.AddRow();
			row.Cells[0].Write("Date of Work Performed:", string.Join(string.Empty, Enumerable.Repeat("_", 12)));
			row.Cells[1].Write("By (Tech) : ", string.Join(string.Empty, Enumerable.Repeat("_", 18)));
			_section.AddParagraph();
		}

		private void WriteCustomerDetails(Workorder workorder)
		{
			var table = Table(8, 8);
			var row = table.AddRow();
			row.Cells[0].Write("CG Facilities Service Call #: ", workorder.WorkOrderNumber);
			row.Cells[0].Write("Store Name: ", workorder.Customer.StoreName);
			row.Cells[1].Write("Customer WO #: ", workorder.CustomerWorkOrderNumber);
			row.Cells[1].Write("Phone: ", workorder.Customer.Telephone);
			row.Cells[0].Borders(true, false, false, true);
			row.Cells[1].Borders(true, true);
			table = Table(16);
			row = table.AddRow();
			row.Cells[0].Write("Store Address: ", workorder.Customer.Address.Full);
			row.Cells[0].Borders(false, true, true, true);
			_section.AddParagraph();
		}

		private void WritePage2Notes()
		{
			_section.Write("Please fill out ONE per visit to site and submit with your invoice.",
				new ParagraphFormat {Alignment = ParagraphAlignment.Center, Font = {Bold = true}});
			_section.Write("No invoice will be processed without a store sign off for each visit.",
				new ParagraphFormat {Alignment = ParagraphAlignment.Center, Font = {Bold = true}});
			_section.AddParagraph();
		}

		private void WritePage1(Workorder workorder)
		{
			WriteLogoAndAddress(workorder);

			WriteDatesAndEmployee(workorder);

			WriteNte(workorder);

			WriteCustomerSiteInformation(workorder);

			WriteBengEndTimes(workorder);

			WriteSow(workorder);

			WriteIvrInfo(workorder);

			WriteNotes(workorder);

			WritePage1Footer();
		}

		private void WriteIvrInfo(Workorder workorder)
		{
			var table = Table(8, 8);
			var row = table.AddRow();
			row.Cells[0].Write("IVR #: ", workorder.IvrNumber);
			row.Cells[1].Write("IVR Code: ", workorder.IvrCode);
			_section.AddParagraph();
		}

		private void WritePage1Footer()
		{
			var p = _section.AddParagraph();
			p.Format.Alignment = ParagraphAlignment.Center;
			p.AddFormattedText("See Page 2 for the Work Order Sign Off Template", TextFormat.Bold);
			_section.AddParagraph();
		}

		private void WriteNotes(Workorder workorder)
		{
			var p = _section.AddParagraph();
			p.AddText($"PLEASE CALL TO CHECK IN AND OUT FROM THE SITE to {"TBD"}. Call ");
			p.AddFormattedText(workorder.Client.Telephone, TextFormat.Bold);
			p.AddText(" IF A CAP INCREASE IS NEEDED WHILE ON SITE.");
			_section.AddParagraph();

			_section.AddParagraph("PLEASE OBTAIN MANAGER ON DUTY SIGNATURE AND THEIR EMPLOYEE UID#.");
			_section.AddParagraph("PLEASE PROVIDE BEFORE AND AFTER PHOTOS");
			_section.AddParagraph("THANK YOU");
			_section.AddParagraph();

			_section.AddParagraph(
				"BY ACCEPTING THIS WORK ORDER, THE SERVICE PARTNER AGREES TO THE TERMS AND CONDITIONS OF THE MSA ON FILE WITH REGARDS TO SERVICE EXPECTATIONS AND PAYMENT TERMS");
			_section.AddParagraph();
		}

		private void WriteSow(Workorder workorder)
		{
			var table = Table(16);
			var row = table.AddRow();
			row.Cells[0].Write(workorder.Sow);
			row.Cells[0].Borders(true, true, true, true);
			row.Height = new Unit(8, UnitType.Centimeter);
			_section.AddParagraph();
		}

		private void WriteBengEndTimes(Workorder workorder)
		{
			var table = Table(10, 6);
			var row = table.AddRow();
			row.Cells[0].Write("Tech must be on site no later than", new ParagraphFormat {Font = {Bold = true}});
			row.Cells[1]
				.Write("Date:",
					$"{workorder.SuggestedBeginDatetime.ToShortDateString()} {workorder.SuggestedBeginDatetime.ToShortTimeString()}");
			row = table.AddRow();
			row.Cells[0].Write("Work must be completed no later than", new ParagraphFormat {Font = {Bold = true}});
			row.Cells[1]
				.Write("Date:",
					$"{workorder.SuggestedEndDatetime.ToShortDateString()} {workorder.SuggestedEndDatetime.ToShortTimeString()}");
			_section.AddParagraph();
		}

		private void WriteNte(Workorder workorder)
		{
			var table = Table(8, 8);
			var row = table.AddRow();
			row.Cells[0].Write("To: ", workorder.Vendor.Name);
			row.Cells[0].Write($"{workorder.Client.Name} service call #: ", workorder.WorkOrderNumber);
			row.Cells[0].Write();
			row.Cells[0].Write("CUSTOMER WO #: ", workorder.CustomerWorkOrderNumber);

			row.Cells[1]
				.Write("NTE AMOUNT: ", workorder.Nte.ToString("C"),
					new ParagraphFormat {Alignment = ParagraphAlignment.Center, Font = {Bold = true}});
			row.Cells[1]
				.Write("THIS IS TOTAL CHARGES TO BE INVOICED ON THIS WORK ORDER.",
					new ParagraphFormat {Alignment = ParagraphAlignment.Center, Font = {Bold = true}});
			row.Cells[1]
				.Write("Exceeding this amount puts you at risk for no payments over the NTE amount.",
					new ParagraphFormat
					{
						Alignment = ParagraphAlignment.Center,
						Font = {Bold = true, Size = new Unit(8, UnitType.Point)}
					});
			row.Cells[1].Borders(true, true, false, true);
		}

		private void WriteCustomerSiteInformation(Workorder workorder)
		{
			var table = Table(8, 8);
			var row = table.AddRow();
			row.Cells[0].Write("CUSTOMER (SITE) INFORMATION");
			row.Cells[0].Write("Store name: ", workorder.Customer.StoreName);
			row.Cells[0].Write("Address: ", workorder.Customer.Address.AddressLines);
			row.Cells[0].Write(workorder.Customer.Address.CityAndStateAndZip);
			row.Cells[0].Write("Site info: ", workorder.Customer.SiteInfo);

			row.Cells[0].Borders.Right.Visible = false;
			row.Cells[1].Borders.Left.Visible = false;

			row.Cells[1].AddParagraph();
			row.Cells[1].AddParagraph();
			row.Cells[1].Write("Phone: ", workorder.Customer.Telephone);
			row.Cells[1].Write("Contact: ", workorder.Customer.ContactPerson);

			row.Cells[0].Borders(true, false, true, true);
			row.Cells[1].Borders(true, true, true);
			_section.AddParagraph();
		}

		private void WriteDatesAndEmployee(Workorder workorder)
		{
			var table = Table(5, 5, 6);
			var row = table.AddRow();
			row.Cells[0].Write("Date: ", workorder.WorkOrderDate.ToShortDateString());
			row.Cells[1].Write("Time: ", workorder.WorkOrderDate.ToShortTimeString());
			row.Cells[2].Write("CSR: ", workorder.Employee.Name);
			_section.AddParagraph();
		}

		private void WriteDocumentHeader()
		{
			var p = _section.Headers.Primary.AddParagraph("WORK ORDER REQUEST");
			p.Format.Alignment = ParagraphAlignment.Center;
		}

		private void WriteLogoAndAddress(Workorder workorder)
		{
			var logoFile = Path.GetTempFileName();
			File.WriteAllBytes(logoFile, workorder.Client.Logo);

			var table = Table(4,12);

			var row = table.AddRow();

			row.Cells[0].AddImage(logoFile);
			row.Cells[1].AddParagraph(workorder.Client.Name);
			row.Cells[1].AddParagraph(workorder.Client.Address.AddressLines);
			row.Cells[1].AddParagraph(workorder.Client.Address.CityAndState);
			row.Cells[1].AddParagraph($"Tel: {workorder.Client.Telephone}");
			row.Cells[1].AddParagraph($"Fax: {workorder.Client.Fax}");
			row.Cells[1].AddParagraph(workorder.Client.WebSite);
		}

		private Table Table(params int[] columns)
		{
			var table = _section.AddTable();
//			table.Borders.Width = 0.25;
//			table.Borders.Color = GetColor("#0055A5");

			foreach (var column in columns)
			{
				table.AddColumn(new Unit(column, UnitType.Centimeter));
			}
			return table;
		}

		private void InitializeDocument()
		{
			_document = new Document
			{
				Info = { Author = "Jacob Inc", Comment = "Jacob Inc", Subject = "Workorder" }
			};

			var style = _document.Styles["Normal"];
			style.Font.Name = "Verdana";
			style.Font.Size = 10;

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