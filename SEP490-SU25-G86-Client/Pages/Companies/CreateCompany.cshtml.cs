using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.IndustryDTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class CreateCompanyModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateCompanyModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        [BindProperty]
        public CompanyCreateUpdateDTO Company { get; set; } = new();

        public List<IndustryDTO> Industries { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER") return RedirectToPage("/Common/Login");

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await _httpClient.GetAsync("api/Industries");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            var userIdStr = HttpContext.Session.GetString("userId");
            var token = HttpContext.Session.GetString("jwt_token");

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdStr) || role != "EMPLOYER")
                return RedirectToPage("/Common/Login");

            if (!int.TryParse(userIdStr, out int userId))
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Load lại ngành nghề nếu có lỗi để giữ lại dropdown khi quay lại trang
            var res = await _httpClient.GetAsync("api/Industries");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Thông tin nhập vào không hợp lệ.";
                return Page();
            }

            var json = JsonSerializer.Serialize(Company);
            var contentJson = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Companies", contentJson);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo công ty thành công!";
                return RedirectToPage("/Companies/CompanyInformation");
            }
            else
            {
                var errMsg = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Tạo công ty thất bại: {errMsg}";
            }

            return Page();
        }
    }
}
