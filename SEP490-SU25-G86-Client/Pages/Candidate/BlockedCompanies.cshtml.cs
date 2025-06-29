using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;

namespace SEP490_SU25_G86_Client.Pages.Candidate
{
    public class BlockedCompaniesModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<BlockedCompanyDTO> BlockedCompanies { get; set; } = new();

        public BlockedCompaniesModel(IHttpClientFactory httpClientFactory)
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