using Aspose.Words;
using Aspose.Words.Drawing;
using System.Reflection;
//using DocumentFormat.OpenXml.Packaging;
//using DocumentFormat.OpenXml.Wordprocessing;
//using System.Text;

namespace DCI.WebApp.Services
{
	public class DocumentService
	{
		//public string ReadWordDocument(string filePath)
		//{
		//	StringBuilder sb = new StringBuilder();

		//	// Open the Word document for reading
		//	using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
		//	{
		//		// Get the main document part
		//		Body body = wordDoc.MainDocumentPart.Document.Body;

		//		// Loop through the paragraphs and extract text
		//		foreach (var paragraph in body.Elements<Paragraph>())
		//		{
		//			sb.AppendLine(paragraph.InnerText);
		//		}
		//	}

		//	// Return the document's text content
		//	return sb.ToString();
		//}
		public string ConvertWordToPdf(string wordFilePath)
		{
			// Load the Word document from the provided file path
			Document doc = new Document(wordFilePath);

			// Generate a PDF file path
			string pdfFilePath = Path.ChangeExtension(wordFilePath, ".pdf");

			// Save the document as PDF
			doc.Save(pdfFilePath, SaveFormat.Pdf);

			// Return the path to the generated PDF
			return pdfFilePath;
		}
		private static void InsertWatermarkIntoHeader(Shape watermark, Section section, HeaderFooterType headerType)
		{
			HeaderFooter header = section.HeadersFooters[headerType];

			// If there is no header of the specified type, create one
			if (header == null)
			{
				header = new HeaderFooter(section.Document, headerType);
				section.HeadersFooters.Add(header);
			}

			// Insert a clone of the watermark into the header
			header.AppendChild(watermark.Clone(true));
		}		

	}
}
