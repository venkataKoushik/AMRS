using System.Diagnostics.Eventing.Reader;
using System.Text;
using NPOI.XWPF.UserModel;
using Tesseract;
using UglyToad.PdfPig;
namespace backendAI.Plugins
{
    public class OCRPlugin
    {
        private static HashSet<string> imageExtensions = new HashSet<string>()
            {
                "png", "jpg", "jpeg", "tif", "tiff", "bmp", "gif", "webp"
            };
        private string tessdataPath = "tessdata";

        // Support multiple languages
        private string languages = "eng+osd";

        public string ExtractText(byte[] fileBytes, string fileName)
        {
            string ext = Path.GetExtension(fileName).Replace(".", "").ToLower();

            if (ext == "pdf")
                
                return ExtractFromPdf(fileBytes);
                   

            if (imageExtensions.Contains(ext))
                return ExtractFromImage(fileBytes);

            return "";
        }
        public string ExtractFromDocx(byte[] fileBytes)
        {
            try
            {
                using (var ms = new MemoryStream(fileBytes))
                {
                    // NPOI opens the byte stream as a Word Document
                    var doc = new XWPFDocument(ms);
                    var text = new StringBuilder();

                    foreach (var para in doc.Paragraphs)
                    {
                        text.AppendLine(para.Text);
                    }

                    return text.ToString();
                }
            }
            catch (Exception ex)
            {
                return $"[Error parsing Docx: {ex.Message}]";
            }
        }

        public string ExtractFromPdf(byte[] pdfBytes)
        {
            try
            {
                using (var document = PdfDocument.Open(pdfBytes))
                {
                    StringBuilder text = new StringBuilder();

                    foreach (var page in document.GetPages())
                    {
                        foreach (var word in page.GetWords())
                        {
                            text.Append(word.Text + " ");
                        }

                        text.AppendLine();
                    }

                    return text.ToString();
                }
            }
            catch
            {
                return "";
            }
        }

        // OCR for images
        public string ExtractFromImage(byte[] imageBytes)
        {
            try
            {
                using (var engine = new TesseractEngine(tessdataPath, languages))
                using (var img = Pix.LoadFromMemory(imageBytes))
                using (var page = engine.Process(img))
                {
                    return page.GetText();
                }
            }
            catch
            {
                return "";
            }
        }
    }
}
