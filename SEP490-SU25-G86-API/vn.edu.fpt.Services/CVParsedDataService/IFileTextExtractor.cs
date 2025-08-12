namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public interface IFileTextExtractor
    {
        Task<string> ExtractTextAsync(IFormFile file, CancellationToken ct = default);
    }
}
