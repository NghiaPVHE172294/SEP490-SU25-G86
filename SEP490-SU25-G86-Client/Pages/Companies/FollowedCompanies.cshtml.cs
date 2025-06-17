using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class FollowedCompaniesModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<CompanyFollowingDTO> Companies { get; set; } = new();

        public FollowedCompaniesModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            int userId = 2; // Hardcode tạm thời, bạn có thể lấy từ token/session sau
            var response = await _httpClient.GetAsync($"api/CompanyFollowers/user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Companies = JsonSerializer.Deserialize<List<CompanyFollowingDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return Page();
        }
    }
}
