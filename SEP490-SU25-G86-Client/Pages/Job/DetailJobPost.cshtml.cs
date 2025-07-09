using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace SEP490_SU25_G86_Client.Pages.Job
{
    public class DetailJobPostModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public ViewDetailJobPostDTO? JobPostDetail { get; set; }
        public string? ErrorMessage { get; set; }
        public List<CvDTO> MyCvs { get; set; } = new List<CvDTO>();

        public DetailJobPostModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new System.Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                // Lấy token từ session
                var token = HttpContext.Session.GetString("jwt_token");
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync($"api/JobPosts/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    JobPostDetail = JsonSerializer.Deserialize<ViewDetailJobPostDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ErrorMessage = "Không tìm thấy tin tuyển dụng.";
                }
                else
                {
                    ErrorMessage = $"Lỗi: {response.StatusCode}";
                }

                // Lấy danh sách CV nếu đã đăng nhập
                var userIdStr = HttpContext.Session.GetString("userId");
                if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
                {
                    var cvRes = await _httpClient.GetAsync("api/Cv/my");
                    if (cvRes.IsSuccessStatusCode)
                    {
                        var cvContent = await cvRes.Content.ReadAsStringAsync();
                        MyCvs = JsonSerializer.Deserialize<List<CvDTO>>(cvContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CvDTO>();
                    }
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