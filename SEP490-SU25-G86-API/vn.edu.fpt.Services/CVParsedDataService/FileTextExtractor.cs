namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public class FileTextExtractor : IFileTextExtractor
    {
        private readonly PdfTextExtractor _pdf;
        private readonly DocxTextExtractor _docx;
        public FileTextExtractor(PdfTextExtractor pdf, DocxTextExtractor docx)
        { _pdf = pdf; _docx = docx; }

        public async Task<string> ExtractTextAsync(IFormFile file, CancellationToken ct = default)
        {
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext == ".pdf") return await _pdf.ExtractAsync(file, ct);
            if (ext == ".docx") return await _docx.ExtractAsync(file, ct);

            throw new NotSupportedException("Chỉ hỗ trợ PDF hoặc DOCX. Vui lòng chuyển .doc sang .docx.");
        }
    }
}
