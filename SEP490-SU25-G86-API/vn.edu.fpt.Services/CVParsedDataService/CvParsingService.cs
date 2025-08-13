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
            var data = JsonSerializer.Deserialize<CvparsedDatum>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new InvalidOperationException("JSON trả về không đúng schema.");

            data.CvId = cvId;
            data.ParsedAt = DateTime.UtcNow;
            data.IsDelete = false;

            await _repo.AddAsync(data, ct);
            return data;
        }
    }
}
