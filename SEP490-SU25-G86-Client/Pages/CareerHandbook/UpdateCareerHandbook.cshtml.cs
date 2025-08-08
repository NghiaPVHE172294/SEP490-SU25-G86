using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;
using System.Text;

namespace SEP490_SU25_G86_Client.Pages.CareerHandbook
{
    public class UpdateCareerHandbookModel : PageModel
    {
        private readonly HttpClient _httpClient;

        [BindProperty]
        public CareerHandbookUpdateDTO Handbook { get; set; } = new();

        public List<SelectListItem> CategorySelectList { get; set; } = new();

        public UpdateCareerHandbookModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var role = HttpContext.Session.GetString("user_role");
            if (string.IsNullOrEmpty(token) || role != "ADMIN")
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Lấy dữ liệu bài viết
            var res = await _httpClient.GetAsync($"api/CareerHandbooks/admin/{id}");
            if (!res.IsSuccessStatusCode)
                return RedirectToPage("/CareerHandbook/ListCareerHandbook");

            var json = await res.Content.ReadAsStringAsync();
            var detail = JsonSerializer.Deserialize<CareerHandbookDetailDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (detail != null)
            {
                Handbook = new CareerHandbookUpdateDTO
                {
                    Title = detail.Title,
                    Slug = detail.Slug,
                    Content = detail.Content,
                    ThumbnailUrl = detail.ThumbnailUrl,
                    Tags = detail.Tags,
                    CategoryId = detail.CategoryId,
                    IsPublished = detail.IsPublished
                };
            }

            // Lấy danh mục
            var resCat = await _httpClient.GetAsync("api/HandbookCategories");
            if (resCat.IsSuccessStatusCode)
            {
                var jsonCat = await resCat.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<dynamic>>(jsonCat, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (categories != null)
                {
                    CategorySelectList = categories.Select(c => new SelectListItem
                    {
                        Value = c.categoryId.ToString(),
                        Text = c.categoryName
                    }).ToList();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(Handbook);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _httpClient.PutAsync($"api/CareerHandbooks/admin/{id}", content);
            if (res.IsSuccessStatusCode)
                return RedirectToPage("/Admin/CareerHandbook/ListCareerHandbook");

            ModelState.AddModelError(string.Empty, "Lỗi khi cập nhật cẩm nang");
            return Page();
        }
    }
}
