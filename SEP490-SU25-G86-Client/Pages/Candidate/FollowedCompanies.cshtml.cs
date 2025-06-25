using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class FollowedCompaniesModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<CompanyFollowingDTO> Companies { get; set; } = new();
        public List<CompanyFollowingDTO> SuggestedCompanies { get; set; } = new();

        public FollowedCompaniesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "CANDIDATE")
            {
                return RedirectToPage("/NotFound");
            }

            var userIdStr = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Common/Login");
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await _httpClient.GetAsync($"api/CompanyFollowers/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Companies = JsonSerializer.Deserialize<List<CompanyFollowingDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
                else
                {
                    Companies = new();
                }
            }
            catch
            {
                Companies = new();
            }

            SuggestedCompanies = new List<CompanyFollowingDTO>
            {
                new CompanyFollowingDTO
                {
                    CompanyId = 301,
                    CompanyName = "Fintech Solutions",
                    IndustryName = "Tài chính - Công nghệ",
                    LogoUrl = "https://via.placeholder.com/40",
                    Description = "Dẫn đầu về giải pháp tài chính số tại Việt Nam."
                },
                new CompanyFollowingDTO
                {
                    CompanyId = 302,
                    CompanyName = "EduWorld",
                    IndustryName = "Giáo dục Quốc tế",
                    LogoUrl = "https://via.placeholder.com/40",
                    Description = "Hệ thống giáo dục toàn cầu với hơn 20 năm kinh nghiệm."
                }
            };

            return Page();
        }

        public async Task<IActionResult> OnPostUnfollowAsync(int followId)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.DeleteAsync($"api/CompanyFollowers/{followId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Đã hủy theo dõi thành công.";
            }
            else
            {
                TempData["Error"] = "Hủy theo dõi thất bại.";
            }

            return RedirectToPage();
        }
    }
}
