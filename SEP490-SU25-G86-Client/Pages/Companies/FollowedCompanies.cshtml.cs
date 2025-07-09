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

            // Lấy danh sách doanh nghiệp đang theo dõi
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

            // Lấy gợi ý doanh nghiệp
            var suggestResponse = await _httpClient.GetAsync($"api/CompanyFollowers/suggest/{userId}?page=1&pageSize=5");
            if (suggestResponse.IsSuccessStatusCode)
            {
                var suggestContent = await suggestResponse.Content.ReadAsStringAsync();
                var suggestResult = JsonSerializer.Deserialize<SuggestedCompanyApiResponse>(suggestContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                SuggestedCompanies = suggestResult?.Companies ?? new();
            }
            else
            {
                SuggestedCompanies = new();
            }

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

        private class SuggestedCompanyApiResponse
        {
            public List<CompanyFollowingDTO> Companies { get; set; } = new();
        }
    }
}
