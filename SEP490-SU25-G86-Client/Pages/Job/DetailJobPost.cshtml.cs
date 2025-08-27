using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
namespace SEP490_SU25_G86_Client.Pages.Job
{
    public class DetailJobPostModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ViewDetailJobPostDTO? JobPostDetail { get; set; }
        public string? ErrorMessage { get; set; }
        public List<CvDTO> MyCvs { get; set; } = new List<CvDTO>();
        public bool IsSaved { get; set; } = false; //  Trạng thái đã lưu hay chưa
        public int? CurrentUserId { get; set; } //  Lưu ID người dùng
        public string? CurrentUserRole { get; set; }
        public List<RelatedJobItemDTO> RelatedJobs { get; set; } = new();
        public DetailJobPostModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new System.Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var token = HttpContext.Session.GetString("jwt_token");
                var userIdStr = HttpContext.Session.GetString("userId");
                var roleStr = HttpContext.Session.GetString("user_role");
                if (!string.IsNullOrEmpty(roleStr))
                {
                    CurrentUserRole = roleStr;
                }

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // 1. Lấy chi tiết tin tuyển dụng
                var response = await _httpClient.GetAsync($"api/JobPosts/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    JobPostDetail = JsonSerializer.Deserialize<ViewDetailJobPostDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (JobPostDetail != null)
                    {
                        var sanitizer = new HtmlSanitizer();
                        sanitizer.AllowedTags.Add("p");
                        sanitizer.AllowedTags.Add("ul");
                        sanitizer.AllowedTags.Add("ol");
                        sanitizer.AllowedTags.Add("li");
                        sanitizer.AllowedTags.Add("strong");
                        sanitizer.AllowedTags.Add("em");
                        sanitizer.AllowedTags.Add("u");
                        sanitizer.AllowedTags.Add("br");
                        sanitizer.AllowedTags.Add("a");
                        sanitizer.AllowedAttributes.Add("href");
                        sanitizer.AllowedSchemes.Add("http");
                        sanitizer.AllowedSchemes.Add("https");

                        JobPostDetail.Description = sanitizer.Sanitize(JobPostDetail.Description ?? "");
                        JobPostDetail.CandidaterRequirements = sanitizer.Sanitize(JobPostDetail.CandidaterRequirements ?? "");
                        JobPostDetail.Interest = sanitizer.Sanitize(JobPostDetail.Interest ?? "");
                    }
                }

                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ErrorMessage = "Không tìm thấy tin tuyển dụng.";
                    return Page();
                }
                else
                {
                    ErrorMessage = $"Lỗi: {response.StatusCode}";
                    return Page();
                }

                // 2. Nếu đã đăng nhập, lấy danh sách CV và kiểm tra trạng thái đã lưu job
                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
                {
                    CurrentUserId = userId;

                    // Lấy danh sách CV
                    var cvRes = await _httpClient.GetAsync("api/Cv/my");
                    if (cvRes.IsSuccessStatusCode)
                    {
                        var cvContent = await cvRes.Content.ReadAsStringAsync();
                        MyCvs = JsonSerializer.Deserialize<List<CvDTO>>(cvContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CvDTO>();
                    }

                    // Deserialize kết quả từ API SavedJobs/check
                    var checkRes = await _httpClient.GetAsync($"api/SavedJobs/check?userId={userId}&jobPostId={id}");
                    if (checkRes.IsSuccessStatusCode)
                    {
                        var savedStr = await checkRes.Content.ReadAsStringAsync();
                        using var doc = JsonDocument.Parse(savedStr);
                        if (doc.RootElement.TryGetProperty("isSaved", out JsonElement value))
                        {
                            IsSaved = value.GetBoolean();
                        }
                    }
                }
                if (JobPostDetail?.IndustryId is int industryId && industryId > 0)
                {
                    var relatedRes = await _httpClient.GetAsync(
                        $"api/JobPosts/related-jobs/{industryId}?take=6&excludeJobPostId={id}");

                    if (relatedRes.IsSuccessStatusCode)
                    {
                        var raw = await relatedRes.Content.ReadAsStringAsync();
                        RelatedJobs = JsonSerializer.Deserialize<List<RelatedJobItemDTO>>(
                            raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                    }
                    // lỗi thì cứ để trống, không cần báo
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
            }

            return Page();
        }
    }
}
