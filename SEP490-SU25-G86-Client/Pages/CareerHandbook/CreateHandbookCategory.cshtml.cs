using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.HandbookCategoryDTO;

namespace SEP490_SU25_G86_Client.Pages.CareerHandbook
{
    public class CreateHandbookCategoryModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public HandbookCategoryCreateDTO Category { get; set; } = new();

        public CreateHandbookCategoryModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(Category);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _httpClient.PostAsync("api/HandbookCategories", content);
            if (res.IsSuccessStatusCode)
                return RedirectToPage("/CareerHandbook/ListHandbookCategory");

            ModelState.AddModelError(string.Empty, "Lỗi khi tạo danh mục");
            return Page();
        }
    }
}
