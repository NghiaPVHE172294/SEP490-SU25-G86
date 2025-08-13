using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public class DocxTextExtractor
    {
        public async Task<string> ExtractAsync(IFormFile file, CancellationToken ct)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            ms.Position = 0;

            using var doc = WordprocessingDocument.Open(ms, false);
            var body = doc.MainDocumentPart!.Document.Body!;
            var sb = new StringBuilder();
            foreach (var t in body.Descendants<Text>())
                sb.Append(t.Text).Append(' ');
            return sb.ToString();
        }
    }
}
