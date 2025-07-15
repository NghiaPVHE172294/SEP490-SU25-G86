using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class CompanyInformationModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CompanyInformationModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public CompanyDetailDTO? Company { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var userIdStr = HttpContext.Session.GetString("userId");
            var role = HttpContext.Session.GetString("user_role");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdStr) || role != "EMPLOYER")
                return RedirectToPage("/Common/Login");

            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/Companies/me/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Company = JsonSerializer.Deserialize<CompanyDetailDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                Company = null;
            }

            return Page();
        }
    }
}
