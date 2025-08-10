using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.CareerHandbook
{
    public class ListModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public ListModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public List<CareerHandbookDetailDTO> CareerHandbooks { get; set; } = new();

        public async Task OnGetAsync()
        {
            var response = await _httpClient.GetAsync("api/CareerHandbooks");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                CareerHandbooks = JsonSerializer.Deserialize<List<CareerHandbookDetailDTO>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }
    }
}
