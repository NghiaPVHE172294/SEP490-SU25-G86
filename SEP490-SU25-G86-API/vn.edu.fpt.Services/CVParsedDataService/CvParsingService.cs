using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVParsedDataRepository;
using System.Text.Json;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public class CvParsingService : ICvParsingService
    {
        private readonly IFileTextExtractor _extractor;
        private readonly IGeminiClient _gemini;
        private readonly ICVParsedDataRepository _repo;

        public CvParsingService(IFileTextExtractor extractor, IGeminiClient gemini, ICVParsedDataRepository repo)
        {
            _extractor = extractor; _gemini = gemini; _repo = repo;
        }

        public async Task<CvparsedDatum> ParseAndSaveAsync(int cvId, IFormFile file, string? prompt, CancellationToken ct = default)
        {
            var rawText = await _extractor.ExtractTextAsync(file, ct);

            // (tùy chọn) cắt còn ~1M tokens – xem mục 10 bên dưới
            var textForModel = TokenLimiter.TrimToApproxTokens(rawText, 1_000_000);

            var defaultPrompt = "Bạn là trình phân tích CV...";
            var jsonString = await _gemini.GenerateJsonAsync(prompt ?? defaultPrompt, textForModel, ct);
            // parse json thành entity
            //var data = JsonSerializer.Deserialize<CvparsedDatum>(jsonString, new JsonSerializerOptions
            //{
            //    PropertyNameCaseInsensitive = true
            //}) ?? throw new InvalidOperationException("JSON trả về không đúng schema.");

            //data.CvId = cvId;
            //data.ParsedAt = DateTime.UtcNow;
            //data.IsDelete = false;

            //await _repo.AddAsync(data, ct);
            //return data;

            if (string.IsNullOrWhiteSpace(jsonString))
                throw new InvalidOperationException("JSON trả về không đúng schema.");

            // parse json thành entity (chuẩn hoá exception)
            CvparsedDatum data;
            try
            {
                data = JsonSerializer.Deserialize<CvparsedDatum>(
                    jsonString,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                ) ?? throw new InvalidOperationException("JSON trả về không đúng schema.");
            }
            catch (OperationCanceledException)
            {
                // tôn trọng cancellation
                throw;
            }
            catch (JsonException ex)
            {
                // JSON sai format/sai kiểu -> chuẩn hoá về InvalidOperationException
                throw new InvalidOperationException("JSON trả về không đúng schema.", ex);
            }

            data.CvId = cvId;
            data.ParsedAt = DateTime.UtcNow;
            data.IsDelete = false;

            await _repo.AddAsync(data, ct);
            return data;
        }
        public async Task<CvparsedDatum> ParseAndSaveFromUrlAsync(int cvId, string fileUrl, string? prompt, CancellationToken ct = default)
        {
            // tải file về RAM
            using var http = new HttpClient();
            using var resp = await http.GetAsync(fileUrl, ct);
            resp.EnsureSuccessStatusCode();
            var ms = new MemoryStream(await resp.Content.ReadAsByteArrayAsync(ct));

            // đoán content-type + tên file từ URL
            var fname = Path.GetFileName(new Uri(fileUrl).AbsolutePath); // có thể là "CV storage/..."
            if (string.IsNullOrEmpty(Path.GetExtension(fname))) fname += ".pdf"; // fallback
            var contentType = resp.Content.Headers.ContentType?.MediaType ?? "application/pdf";

            // wrap thành IFormFile để xài extractor hiện có
            IFormFile formFile = new FormFile(ms, 0, ms.Length, "file", fname)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return await ParseAndSaveAsync(cvId, formFile, prompt, ct);
        }
    }
}
