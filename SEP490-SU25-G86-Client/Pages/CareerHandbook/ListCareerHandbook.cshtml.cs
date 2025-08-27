using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;

namespace SEP490_SU25_G86_Client.Pages.CarrerHandbook
{
    public class ListCareerHandbookModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<CareerHandbookDetailDTO> Handbooks { get; set; } = new();
        public List<CareerHandbookDetailDTO> PagedHandbooks { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;

        public int TotalPages { get; set; }

        public ListCareerHandbookModel(IHttpClientFactory httpClientFactory)
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

            var res = await _httpClient.GetAsync("api/CareerHandbooks/admin");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var allHandbooks = JsonSerializer.Deserialize<List<CareerHandbookDetailDTO>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                // Lọc search
                if (!string.IsNullOrEmpty(Search))
                {
                    allHandbooks = allHandbooks
                        .Where(h => h.Title.Contains(Search, StringComparison.OrdinalIgnoreCase)
                                 || h.CategoryName.Contains(Search, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                Handbooks = allHandbooks;

                // Pagination
                if (PageSize <= 0) PageSize = 5;
                var totalItems = Handbooks.Count;
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

                if (Page < 1) Page = 1;
                if (Page > TotalPages && TotalPages > 0) Page = TotalPages;

                PagedHandbooks = Handbooks
                    .OrderByDescending(h => h.CreatedAt)
                    .Skip((Page - 1) * PageSize)
                    .Take(PageSize)
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var role = HttpContext.Session.GetString("user_role");
            if (string.IsNullOrEmpty(token) || role != "ADMIN")
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _httpClient.DeleteAsync($"api/CareerHandbooks/admin/{id}");
            if (res.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Xóa bài viết thành công";
            }
            else
            {
                TempData["ErrorMessage"] = "Xóa thất bại";
            }

            return RedirectToPage(new { search = Search, page = Page, pageSize = PageSize });
        }
    }
}
