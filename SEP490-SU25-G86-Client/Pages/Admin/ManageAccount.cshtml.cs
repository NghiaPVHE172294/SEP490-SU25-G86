using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AdminAccountDTO;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Admin
{
    public class ManageAccountModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<AccountDTOForList> AccountList { get; set; } = new();

        public ManageAccountModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<IActionResult> OnGetAsync(string? role, string? accountName)
        {
            var roleForAuthen = HttpContext.Session.GetString("user_role");
            if (roleForAuthen != "ADMIN")
            {
                return RedirectToPage("/NotFound");
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Login"); // hoặc xử lý tùy bạn
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            string apiUrl = "https://localhost:7004/api/AdminAccount";

            if (!string.IsNullOrEmpty(role) && role.ToUpper() != "ALL")
            {
                apiUrl += $"/role/{role}";
            }

            if (!string.IsNullOrEmpty(accountName))
            {
                apiUrl += apiUrl.Contains("?") ? "&" : "?";
                apiUrl += $"name={accountName}";
            }

            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                AccountList = JsonSerializer.Deserialize<List<AccountDTOForList>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<AccountDTOForList>();
            }

            return Page();
        }

    }
}
