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

        // 🔹 Thêm property này để dùng trong Razor
        public List<CareerHandbookDetailDTO> SuggestedHandbooks { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return RedirectToPage("/CareerHandbook/List");

            // Gọi API lấy chi tiết bài viết
            var response = await _httpClient.GetAsync($"api/CareerHandbooks/view/{slug}");
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var json = await response.Content.ReadAsStringAsync();
            CareerHandbook = JsonSerializer.Deserialize<CareerHandbookDetailDTO>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (CareerHandbook == null)
                return NotFound();

            // Gọi API lấy danh sách để hiển thị 3 bài gợi ý
            var listResponse = await _httpClient.GetAsync("api/CareerHandbooks");
            if (listResponse.IsSuccessStatusCode)
            {
                var listJson = await listResponse.Content.ReadAsStringAsync();
                var allHandbooks = JsonSerializer.Deserialize<List<CareerHandbookDetailDTO>>(listJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CareerHandbookDetailDTO>();

                // Lọc bỏ bài hiện tại
                var otherPosts = allHandbooks.Where(h => h.Slug != slug).ToList();

                // Lấy 3 bài mới nhất
                SuggestedHandbooks = otherPosts
                    .OrderByDescending(h => h.CreatedAt)
                    .Take(3)
                    .ToList();
            }

            return Page();
        }
    }
}
