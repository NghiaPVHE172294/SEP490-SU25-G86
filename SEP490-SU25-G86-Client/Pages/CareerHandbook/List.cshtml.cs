using Microsoft.AspNetCore.Mvc;
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

        // 🔹 Pagination props
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 6;
        public int TotalPages { get; set; }
        public List<CareerHandbookDetailDTO> PagedHandbooks { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var response = await _httpClient.GetAsync("api/CareerHandbooks");
            if (!response.IsSuccessStatusCode)
            {
                CareerHandbooks = new();
                return Page();
            }

            var json = await response.Content.ReadAsStringAsync();
            CareerHandbooks = JsonSerializer.Deserialize<List<CareerHandbookDetailDTO>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            // Sort mới nhất
            CareerHandbooks = CareerHandbooks.OrderByDescending(h => h.CreatedAt).ToList();

            // Pagination logic
            if (int.TryParse(HttpContext.Request.Query["page"], out int page))
                Page = page > 0 ? page : 1;

            TotalPages = (int)Math.Ceiling(CareerHandbooks.Count / (double)PageSize);
            PagedHandbooks = CareerHandbooks.Skip((Page - 1) * PageSize).Take(PageSize).ToList();

            return Page();
        }
    }
}
