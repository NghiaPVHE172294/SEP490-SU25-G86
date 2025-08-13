using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;

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

        public class HandbookCategorySimpleDTO
        {
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var role = HttpContext.Session.GetString("user_role");
            if (string.IsNullOrEmpty(token) || role != "ADMIN")
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Lấy dữ liệu handbook
            var res = await _httpClient.GetAsync($"api/CareerHandbooks/{id}");
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

            // Lấy danh sách category
            var resCat = await _httpClient.GetAsync("api/HandbookCategories");
            if (resCat.IsSuccessStatusCode)
            {
                var jsonCat = await resCat.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<HandbookCategorySimpleDTO>>(jsonCat, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (categories != null)
                {
                    CategorySelectList = categories.Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var content = new MultipartFormDataContent();

            content.Add(new StringContent(Handbook.Title ?? ""), "Title");
            content.Add(new StringContent(Handbook.Slug ?? ""), "Slug");
            content.Add(new StringContent(Handbook.Content ?? ""), "Content");
            content.Add(new StringContent(Handbook.Tags ?? ""), "Tags");
            content.Add(new StringContent(Handbook.CategoryId.ToString()), "CategoryId");
            content.Add(new StringContent(Handbook.IsPublished.ToString()), "IsPublished");

            if (Handbook.ThumbnailFile != null)
            {
                var fileContent = new StreamContent(Handbook.ThumbnailFile.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(Handbook.ThumbnailFile.ContentType);
                content.Add(fileContent, "ThumbnailFile", Handbook.ThumbnailFile.FileName);
            }
            else
            {
                // Nếu không chọn ảnh mới thì gửi URL ảnh cũ
                content.Add(new StringContent(Handbook.ThumbnailUrl ?? ""), "ThumbnailUrl");
            }

            var res = await _httpClient.PutAsync($"api/CareerHandbooks/{id}", content);
            if (res.IsSuccessStatusCode)
                return RedirectToPage("/CareerHandbook/ListCareerHandbook");

            var errorMsg = await res.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Lỗi khi cập nhật cẩm nang: {errorMsg}");
            return Page();
        }
    }
}
