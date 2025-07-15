using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class CompanyInformationModel : PageModel
    {
        private readonly HttpClient _httpClient;

        // Constructor: khởi tạo HttpClient với base URL
        public CompanyInformationModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        // Thuộc tính để binding dữ liệu công ty trả về từ API
        public CompanyDetailDTO? Company { get; set; }

        // Xử lý khi gọi trang (HTTP GET)
        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy token và role từ Session
            var token = HttpContext.Session.GetString("jwt_token");
            var role = HttpContext.Session.GetString("user_role");

            // Nếu chưa đăng nhập hoặc không phải EMPLOYER, chuyển đến trang đăng nhập
            if (string.IsNullOrEmpty(token) || role != "EMPLOYER")
                return RedirectToPage("/Common/Login");

            // Gắn token vào Header để gọi API bảo vệ
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API để lấy thông tin công ty do người dùng hiện tại tạo
            var response = await _httpClient.GetAsync("api/Companies/me");

            // Nếu lấy thành công thì deserialize dữ liệu, ngược lại gán Company = null
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Company = JsonSerializer.Deserialize<CompanyDetailDTO>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            else
            {
                Company = null;
            }

            return Page();
        }
    }
}
