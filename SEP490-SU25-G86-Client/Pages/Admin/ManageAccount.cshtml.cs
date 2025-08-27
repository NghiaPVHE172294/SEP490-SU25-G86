using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AdminAccountDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Admin
{
    public class ManageAccountModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<AccountDTOForList> AccountList { get; set; } = new();
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
        public string? CurrentRole { get; set; }
        public string? CurrentName { get; set; }

        public ManageAccountModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> OnGetAsync(string? role, string? accountName, int pageNumber = 1, int pageSize = 10)
        {
            var roleForAuthen = HttpContext.Session.GetString("user_role");
            if (roleForAuthen != "ADMIN") return RedirectToPage("/NotFound");

            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Login");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            CurrentRole = string.IsNullOrEmpty(role) ? "ALL" : role;
            CurrentName = accountName;

            var apiUrl = $"https://localhost:7004/api/AdminAccount?pageNumber={pageNumber}&pageSize={pageSize}";

            if (!string.IsNullOrEmpty(role)) apiUrl += $"&role={Uri.EscapeDataString(role)}";
            if (!string.IsNullOrEmpty(accountName)) apiUrl += $"&name={Uri.EscapeDataString(accountName)}";

            var response = await _httpClient.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var paged = JsonSerializer.Deserialize<PagedAccountResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (paged != null)
                {
                    AccountList = paged.Items ?? new();
                    PageNumber = paged.PageNumber;
                    TotalPages = paged.TotalPages;
                    HasPrevious = paged.HasPrevious;
                    HasNext = paged.HasNext;
                }
            }

            return Page();
        }

    }
}
