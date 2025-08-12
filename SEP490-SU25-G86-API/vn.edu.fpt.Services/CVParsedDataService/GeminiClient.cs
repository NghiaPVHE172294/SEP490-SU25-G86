using System.Text.Json;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService
{
    public class GeminiClient : IGeminiClient
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        public GeminiClient(IHttpClientFactory httpFactory, IConfiguration config)
        { _httpFactory = httpFactory; _config = config; }

        public async Task<string> GenerateJsonAsync(string prompt, string text, CancellationToken ct = default)
        {
            var apiKey = _config["Gemini2:ApiKey"] ?? throw new InvalidOperationException("Missing Gemini2:ApiKey");
            var model = _config["Gemini2:Model"] ?? "gemini-2.0-flash";
            var client = _httpFactory.CreateClient("Gemini2");

            // generationConfig để yêu cầu JSON
            var body = new
            {
                contents = new[]
                {
                new {
                  role = "user",
                  parts = new object[] {
                      new { text = $"{prompt}\n\n===\n{text}" }
                  }
                }
            },
                generationConfig = new
                {
                    response_mime_type = "application/json",
                    temperature = 0.2
                }
            };

            var url = $"{client.BaseAddress}?key={apiKey}";
            using var resp = await client.PostAsJsonAsync(url, body, cancellationToken: ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);

            // Gemini trả { candidates: [ { content: { parts: [ { text: "..." } ] } } ] }
            using var doc = JsonDocument.Parse(json);
            var textNode = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(textNode))
                throw new InvalidOperationException("Gemini không trả text");

            return textNode!;
        }
    }
}
