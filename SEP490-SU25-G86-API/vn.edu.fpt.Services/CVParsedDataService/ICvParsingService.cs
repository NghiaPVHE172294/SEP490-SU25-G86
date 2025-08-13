using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public interface ICvParsingService
    {
        Task<CvparsedDatum> ParseAndSaveAsync(int cvId, IFormFile file, string? prompt, CancellationToken ct = default);
    }
}
