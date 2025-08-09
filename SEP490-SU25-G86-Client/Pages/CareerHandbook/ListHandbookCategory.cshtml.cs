using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.HandbookCategoryDTO;

namespace SEP490_SU25_G86_Client.Pages.CareerHandbook
{
    public class ListHandbookCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<HandbookCategoryDetailDTO> Categories { get; set; } = new();

        public ListHandbookCategoryModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var role = HttpContext.Session.GetString("user_role");
            if (string.IsNullOrEmpty(token) || role != "ADMIN")
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _httpClient.GetAsync("api/HandbookCategories");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                Categories = JsonSerializer.Deserialize<List<HandbookCategoryDetailDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _httpClient.DeleteAsync($"api/HandbookCategories/{id}");
            if (res.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa danh mục thành công";
            }
            else
            {
                TempData["Error"] = await res.Content.ReadAsStringAsync();
            }

            return RedirectToPage();
        }
    }
}
