using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.HandbookCategoryDTO;

namespace SEP490_SU25_G86_Client.Pages.CareerHandbook
{
    public class UpdateHandbookCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public HandbookCategoryUpdateDTO Category { get; set; } = new();

        public UpdateHandbookCategoryModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _httpClient.GetAsync($"api/HandbookCategories/{id}");
            if (!res.IsSuccessStatusCode)
                return RedirectToPage("/CareerHandbook/ListHandbookCategory");

            var json = await res.Content.ReadAsStringAsync();
            var detail = JsonSerializer.Deserialize<HandbookCategoryDetailDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (detail != null)
            {
                Category = new HandbookCategoryUpdateDTO
                {
                    CategoryName = detail.CategoryName,
                    Description = detail.Description
                };
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(Category);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _httpClient.PutAsync($"api/HandbookCategories/{id}", content);
            if (res.IsSuccessStatusCode)
                return RedirectToPage("/CareerHandbook/ListHandbookCategory");

            ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật danh mục");
            return Page();
        }
    }
}
