using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;

namespace SEP490_SU25_G86_Client.Pages.Candidate
{
    public class SuggestedCompanyApiResponse
    {
        public List<CompanyFollowingDTO> Companies { get; set; } = new();
    }

    public class BlockedCompaniesModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<BlockedCompanyDTO> BlockedCompanies { get; set; } = new();
        public List<CompanyFollowingDTO> SuggestedCompanies { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalSuggestions { get; set; } = 0;

        public BlockedCompaniesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync(int page = 1)
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

            CurrentPage = page > 0 ? page : 1;

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/BlockedCompanies/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                BlockedCompanies = JsonSerializer.Deserialize<List<BlockedCompanyDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<BlockedCompanyDTO>();
            }
            else
            {
                BlockedCompanies = new List<BlockedCompanyDTO>();
            }

            // Lấy gợi ý công ty (backend đã loại trừ công ty đã follow và bị block)
            var suggestResponse = await _httpClient.GetAsync($"api/CompanyFollowers/suggest/{userId}?page={CurrentPage}&pageSize={PageSize}");
            if (suggestResponse.IsSuccessStatusCode)
            {
                var suggestContent = await suggestResponse.Content.ReadAsStringAsync();
                var suggestResult = JsonSerializer.Deserialize<SuggestedCompanyApiResponse>(suggestContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                SuggestedCompanies = suggestResult?.Companies ?? new List<CompanyFollowingDTO>();
                
                // Lấy tổng số công ty để tính phân trang (gọi API với pageSize lớn để đếm)
                var countResponse = await _httpClient.GetAsync($"api/CompanyFollowers/suggest/{userId}?page=1&pageSize=1000");
                if (countResponse.IsSuccessStatusCode)
                {
                    var countContent = await countResponse.Content.ReadAsStringAsync();
                    var countResult = JsonSerializer.Deserialize<SuggestedCompanyApiResponse>(countContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    TotalSuggestions = countResult?.Companies?.Count ?? 0;
                }
                else
                {
                    TotalSuggestions = SuggestedCompanies.Count;
                }
                
                TotalPages = (int)Math.Ceiling((double)TotalSuggestions / PageSize);
            }
            else
            {
                SuggestedCompanies = new();
                TotalSuggestions = 0;
                TotalPages = 1;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostUnblockAsync(int blockedCompanyId)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"api/BlockedCompanies/{blockedCompanyId}");
            // Sau khi gỡ block, reload lại trang
            return RedirectToPage();
        }
    }
} 