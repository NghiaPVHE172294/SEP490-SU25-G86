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

        public CompanyInformationModel(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public CompanyDetailDTO? Company { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return RedirectToPage("/Common/Login");

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _httpClient.GetAsync($"api/Companies/user/{userId}");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                Company = JsonSerializer.Deserialize<CompanyDetailDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return Page();
        }
    }
}
