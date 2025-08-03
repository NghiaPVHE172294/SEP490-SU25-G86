using Newtonsoft.Json;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ParseCvRepository;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.ParseCvService
{
    public class ParseCvService : IParseCvService
    {
        private readonly IConfiguration _config;
        private readonly IParseCvRepository _repo;
        private readonly IHttpClientFactory _httpFactory;

        public ParseCvService(
            IConfiguration config,
            IParseCvRepository repo,
            IHttpClientFactory httpFactory)
        {
            _config = config;
            _repo = repo;
            _httpFactory = httpFactory;
        }

        public async Task<CvparsedDatum> ParseAndSaveAsync(IFormFile file)
        {
            // 1. Read file to text
            string text = file.FileName.EndsWith(".pdf")
                ? PdfToText(file.OpenReadStream())
                : DocxToText(file.OpenReadStream());

            // 2. Call OpenAI
            string json = await ExtractCvInfoWithOpenAI(text);

            // 3. Deserialize
            var parsed = JsonConvert.DeserializeObject<CvparsedDatum>(json)
                         ?? throw new InvalidOperationException("Invalid JSON");

            // 4. (Optional) assign CvId here if needed
            parsed.CvId = 36;

            // 5. Save
            await _repo.AddAsync(parsed);
            return parsed;
        }

        // PdfToText, DocxToText, ExtractCvInfoWithOpenAI methods...
        private string PdfToText(Stream pdfStream)
        {
            using var reader = new iText.Kernel.Pdf.PdfReader(pdfStream);
            using var pdfDoc = new iText.Kernel.Pdf.PdfDocument(reader);
            var sb = new StringBuilder();

            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);
                sb.Append(
                  iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor
                       .GetTextFromPage(page));
            }

            // Luôn return, kể cả khi file rỗng
            return sb.ToString();
        }
        private string DocxToText(Stream docxStream)
        {
            // Copy vào MemoryStream để DocX có thể load lại từ đầu
            using var ms = new MemoryStream();
            docxStream.CopyTo(ms);
            ms.Position = 0;

            using var document = Xceed.Words.NET.DocX.Load(ms);
            // DocX.Text luôn trả về ít nhất chuỗi rỗng
            return document.Text ?? string.Empty;
        }

        private string GetPromptFromFile(string cvText)
        {
            // Giả sử file để ở cùng Solution với Program.cs
            var promptPath = Path.Combine(AppContext.BaseDirectory, "promptGPT.txt");
            var template = File.ReadAllText(promptPath);
            return template.Replace("{cvText}", cvText);
        }

        private string GetPromptTemplate()
        {
            // Đường dẫn tuyệt đối, lấy đúng file .txt đã copy vào output folder khi build/publish
            var filePath = Path.Combine(AppContext.BaseDirectory, "promptGPT.txt");
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Không tìm thấy file prompt: {filePath}");

            return File.ReadAllText(filePath);
        }

        private async Task<string> ExtractCvInfoWithOpenAI(string cvText)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            // Đọc template từ file prompt và inject CV text
            var promptTemplate = GetPromptTemplate();
            var prompt = promptTemplate.Replace("{cvText}", cvText);

            var request = new
            {
                model = "gpt-4o",
                messages = new[]
                {
            new { role = "system", content = "Bạn là AI trích xuất thông tin từ CV" },
            new { role = "user", content = prompt }
        },
                max_tokens = 2048,
                temperature = 0
            };

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            // Gửi request tới OpenAI
            var resp = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);

            // Bắt lỗi rate limit
            if (resp.StatusCode == (HttpStatusCode)429)
            {
                if (resp.Headers.RetryAfter?.Delta is TimeSpan delay)
                    await Task.Delay(delay);

                throw new HttpRequestException("OpenAI rate limit exceeded. Vui lòng thử lại sau.");
            }

            resp.EnsureSuccessStatusCode();

            var str = await resp.Content.ReadAsStringAsync();
            dynamic jr = JsonConvert.DeserializeObject(str);

            // Bắt trường hợp không có trường message/content (phòng lỗi lạ)
            if (jr?.choices == null || jr.choices.Count == 0 || jr.choices[0]?.message?.content == null)
                throw new InvalidOperationException("OpenAI trả về dữ liệu không hợp lệ!");

            return jr.choices[0].message.content;
        }

    }
}
