using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public interface ICvParsingService
    {
        Task<CvparsedDatum> ParseAndSaveAsync(int cvId, IFormFile file, string? prompt, CancellationToken ct = default);

        // thêm mới để parse từ file URL (đã upload lên Firebase)
        Task<CvparsedDatum> ParseAndSaveFromUrlAsync(int cvId, string fileUrl, string? prompt, CancellationToken ct = default);
    }
}
