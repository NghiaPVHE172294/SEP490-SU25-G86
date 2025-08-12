namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public interface IGeminiClient
    {
        Task<string> GenerateJsonAsync(string prompt, string text, CancellationToken ct = default);
    }
}
