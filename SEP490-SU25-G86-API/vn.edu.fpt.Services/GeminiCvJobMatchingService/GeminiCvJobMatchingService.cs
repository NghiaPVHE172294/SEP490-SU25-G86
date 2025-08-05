using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SEP490_SU25_G86_API.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SEP490_SU25_G86_API.Services.GeminiCvJobMatchingService
{
    public class GeminiCvJobMatchingService : IGeminiCvJobMatchingService
    {
        private readonly SEP490_G86_CvMatchContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public GeminiCvJobMatchingService(SEP490_G86_CvMatchContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<MatchedCvandJobPost?> CompareCvWithJobCriteriaAsync(int cvParsedDataId, int jobCriteriaId)
        {
            var cv = await _context.CvparsedData.FirstOrDefaultAsync(x => x.CvparsedDataId == cvParsedDataId);
            var criteria = await _context.JobCriteria.FirstOrDefaultAsync(x => x.JobCriteriaId == jobCriteriaId);
            if (cv == null || criteria == null)
                throw new Exception("CV hoặc Job Criteria không tồn tại");

            // Chuẩn bị prompt cho Gemini
            var prompt = BuildPrompt(cv, criteria);
            var geminiApiKey = _configuration["Gemini:ApiKey"];
            // Sử dụng model mới, endpoint mới
            var geminiEndpoint = _configuration["Gemini:Endpoint"] ?? "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
            var httpClient = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                }
            };
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var request = new HttpRequestMessage(HttpMethod.Post, geminiEndpoint)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };
            // Truyền API key qua header thay vì query string
            request.Headers.Add("X-goog-api-key", geminiApiKey);

            // Gửi request tới Gemini
            var response = await httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Log prompt/response
            await LogGeminiRequestAsync(cvParsedDataId, jobCriteriaId, prompt, responseContent);

            // Parse response từ Gemini
            var matched = ParseGeminiResponse(responseContent, cvParsedDataId, jobCriteriaId);

            // Lưu vào DB
            _context.MatchedCvandJobPosts.Add(matched);
            await _context.SaveChangesAsync();

            // Sau khi lưu matched, cập nhật lại Cvsubmission liên quan
            // Tìm Cvsubmission theo CvId (từ cv.CvId) và JobPostId (từ criteria.JobPostId)
            var submission = await _context.Cvsubmissions.FirstOrDefaultAsync(s => s.CvId == cv.CvId && s.JobPostId == criteria.JobPostId && !s.IsDelete);
            if (submission != null)
            {
                submission.MatchedCvandJobPostId = matched.MatchedCvandJobPostId;
                submission.Status = "Đã chấm điểm bằng AI";
                await _context.SaveChangesAsync();
            }
            return matched;
        }

        private string BuildPrompt(CvparsedDatum cv, JobCriterion criteria)
        {
            var prompt = $@"
Bạn là AI đánh giá mức độ phù hợp giữa CV ứng viên và tiêu chí tuyển dụng.

Yêu cầu:
- Đọc dữ liệu CV và Job Criteria bên dưới (dạng JSON).
- So sánh từng tiêu chí tương ứng: kinh nghiệm (experience), kỹ năng (skills), học vấn (education level), chức danh (job titles), ngôn ngữ (languages), chứng chỉ (certifications), tóm tắt (summary), lịch sử làm việc (work history), dự án (projects).
- Cho điểm từng mục từ 0 đến 10 (0: không phù hợp hoặc thiếu/null, 10: rất phù hợp, có thể dùng số thực).
- Nếu trường trong CV bị thiếu/null, trả về điểm 0 cho trường đó.
- Tổng điểm (totalScore) là trung bình cộng tất cả các mục trên (bao gồm cả các trường bị 0).
- Chỉ trả về đúng format JSON như ví dụ, không thêm text ngoài JSON.

Ví dụ format JSON:
{{
  ""experienceScore"": 8.5,
  ""skillsScore"": 7,
  ""educationLevelScore"": 9,
  ""jobTitlesScore"": 8,
  ""languagesScore"": 7.5,
  ""certificationsScore"": 6,
  ""summaryScore"": 8,
  ""workHistoryScore"": 9,
  ""projectsScore"": 7,
  ""totalScore"": 7.78
}}

Dữ liệu:
CV: {JsonSerializer.Serialize(cv)}
JobCriteria: {JsonSerializer.Serialize(criteria)}";

            return prompt;
        }

        private MatchedCvandJobPost ParseGeminiResponse(string responseContent, int cvParsedDataId, int jobCriteriaId)
        {
            using var doc = JsonDocument.Parse(responseContent);
            JsonElement root = doc.RootElement;

            // Nếu có trường error => throw exception rõ ràng
            if (root.TryGetProperty("error", out var errorElem))
            {
                var errorMsg = errorElem.TryGetProperty("message", out var msgElem) ? msgElem.GetString() : errorElem.ToString();
                throw new Exception("Gemini API error: " + errorMsg);
            }

            // Nếu response là candidates/parts/text (format Gemini mới)
            if (root.TryGetProperty("candidates", out var candidatesElem) && candidatesElem.ValueKind == JsonValueKind.Array)
            {
                var candidate = candidatesElem[0];
                if (candidate.TryGetProperty("content", out var contentElem) && contentElem.TryGetProperty("parts", out var partsElem) && partsElem.ValueKind == JsonValueKind.Array)
                {
                    var text = partsElem[0].GetProperty("text").GetString();
                    // Loại bỏ markdown code block nếu có
                    if (text.StartsWith("```json"))
                    {
                        text = text.Substring(7);
                    }
                    if (text.StartsWith("```"))
                    {
                        text = text.Substring(3);
                    }
                    text = text.Trim();
                    if (text.EndsWith("```"))
                    {
                        text = text.Substring(0, text.Length - 3).Trim();
                    }
                    root = JsonDocument.Parse(text).RootElement;
                }
            }
            else if (root.TryGetProperty("text", out var textElem))
            {
                root = JsonDocument.Parse(textElem.GetString()).RootElement;
            }

            // Hàm lấy điểm an toàn, nếu thiếu trả về 0
            double GetScore(string key)
                => root.TryGetProperty(key, out var prop) && prop.TryGetDouble(out var val) ? val : 0.0;

            var matched = new MatchedCvandJobPost
            {
                CvparsedDataId = cvParsedDataId,
                JobPostCriteriaId = jobCriteriaId,
                ExperienceScore = GetScore("experienceScore"),
                SkillsScore = GetScore("skillsScore"),
                EducationLevelScore = GetScore("educationLevelScore"),
                JobTitlesScore = GetScore("jobTitlesScore"),
                LanguagesScore = GetScore("languagesScore"),
                CertificationsScore = GetScore("certificationsScore"),
                SummaryScore = GetScore("summaryScore"),
                WorkHistoryScore = GetScore("workHistoryScore"),
                ProjectsScore = GetScore("projectsScore"),
                
                
                TotalScore = GetScore("totalScore"),
            };
            return matched;
        }

        private async Task LogGeminiRequestAsync(int cvParsedDataId, int jobCriteriaId, string prompt, string response)
        {
            // Ghi log ra file hoặc bảng DB tuỳ nhu cầu
            var log = $"[{DateTime.Now}] CV: {cvParsedDataId}, JobCriteria: {jobCriteriaId}\nPrompt: {prompt}\nResponse: {response}\n";
            var logPath = _configuration["Gemini:LogFile"] ?? "LogAPI_AI/GeminiLog.txt";
            await System.IO.File.AppendAllTextAsync(logPath, log);
        }
    }
}
