using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.CareerHandbook
{
    public class ViewCareerHandbookModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public CareerHandbookDetailDTO? Handbook { get; set; }

        public ViewCareerHandbookModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            var res = await _httpClient.GetAsync($"api/CareerHandbooks/{slug}");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                Handbook = JsonSerializer.Deserialize<CareerHandbookDetailDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return Page();
        }
    }
}
