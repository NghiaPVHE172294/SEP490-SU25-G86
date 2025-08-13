using System.Text;
using UglyToad.PdfPig;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public class PdfTextExtractor
    {
        public async Task<string> ExtractAsync(IFormFile file, CancellationToken ct)
        {
            await using var ms = new MemoryStream();
            await file.CopyToAsync(ms, ct);
            ms.Position = 0;

            var sb = new StringBuilder(1024 * 1024);
            using var pdf = PdfDocument.Open(ms);
            foreach (var page in pdf.GetPages())
            {
                sb.AppendLine(page.Text);
            }
            return sb.ToString();
        }
    }
}
