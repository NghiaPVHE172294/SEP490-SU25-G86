using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.CareerHandbook
{
    public class DetailModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public DetailModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public CareerHandbookDetailDTO? CareerHandbook { get; set; }

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return RedirectToPage("/CareerHandbook/List");

            var response = await _httpClient.GetAsync($"api/CareerHandbooks/view/{slug}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            CareerHandbook = JsonSerializer.Deserialize<CareerHandbookDetailDTO>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (CareerHandbook == null)
                return NotFound();

            return Page();
        }
    }
}
