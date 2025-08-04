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
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalPages { get; set; }
        public int TotalFollowed { get; set; }
        // Suggestion pagination
        public int CurrentSuggestPage { get; set; } = 1;
        public int SuggestPageSize { get; set; } = 6;
        public int TotalSuggestPages { get; set; }
        public int TotalSuggested { get; set; }

        public FollowedCompaniesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync(int page = 1, int suggestPage = 1)
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

            CurrentPage = page;
            CurrentSuggestPage = suggestPage;

            // Lấy danh sách doanh nghiệp đang theo dõi (phân trang)
            var response = await _httpClient.GetAsync($"api/CompanyFollowers/user/{userId}?page={CurrentPage}&pageSize={PageSize}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonSerializer.Deserialize<FollowedCompanyApiResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                Companies = pagedResult?.Companies ?? new();
                TotalFollowed = pagedResult?.Total ?? 0;
                TotalPages = (int)Math.Ceiling((double)TotalFollowed / PageSize);
            }
            else
            {
                Companies = new();
                TotalPages = 1;
            }

            // Lấy gợi ý doanh nghiệp (phân trang)
            var suggestResponse = await _httpClient.GetAsync($"api/CompanyFollowers/suggest/{userId}?page={CurrentSuggestPage}&pageSize={SuggestPageSize}");
            if (suggestResponse.IsSuccessStatusCode)
            {
                var suggestContent = await suggestResponse.Content.ReadAsStringAsync();
                var suggestResult = JsonSerializer.Deserialize<SuggestedCompanyApiResponse>(suggestContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                SuggestedCompanies = suggestResult?.Companies ?? new();
                TotalSuggested = suggestResult?.Total ?? 0;
                TotalSuggestPages = (int)Math.Ceiling((double)TotalSuggested / SuggestPageSize);
            }
            else
            {
                SuggestedCompanies = new();
                TotalSuggestPages = 1;
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
            public int Total { get; set; }
        }
        private class FollowedCompanyApiResponse
        {
            public List<CompanyFollowingDTO> Companies { get; set; } = new();
            public int Total { get; set; }
        }
    }
}
