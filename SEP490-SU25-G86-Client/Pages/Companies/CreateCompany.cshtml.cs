using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.IndustryDTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class CreateCompanyModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public CompanyCreateUpdateDTO Company { get; set; } = new();

        public List<IndustryDTO> Industries { get; set; } = new();

        public CreateCompanyModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Load danh sách ngành nghề
            var res = await _httpClient.GetAsync("api/Industries");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(Company);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Companies", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("CompanyInformation");
            }

            TempData["Error"] = "Tạo công ty thất bại.";
            return Page();
        }
    }
}
