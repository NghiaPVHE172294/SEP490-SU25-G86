using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.CareerHandbooks
{
    public class CreateCareerHandbookModel : PageModel
    {
        private readonly HttpClient _httpClient;
        [BindProperty]
        public CareerHandbookCreateDTO Handbook { get; set; } = new();
        public List<SelectListItem> CategorySelectList { get; set; } = new();

        public CreateCareerHandbookModel(IHttpClientFactory httpClientFactory)
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
                var categories = JsonSerializer.Deserialize<List<dynamic>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(Handbook);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var res = await _httpClient.PostAsync("api/CareerHandbooks/admin", content);
            if (res.IsSuccessStatusCode)
                return RedirectToPage("/Admin/CareerHandbooks/ListCareerHandbook");

            ModelState.AddModelError(string.Empty, "Lỗi khi thêm cẩm nang");
            return Page();
        }
    }
}
