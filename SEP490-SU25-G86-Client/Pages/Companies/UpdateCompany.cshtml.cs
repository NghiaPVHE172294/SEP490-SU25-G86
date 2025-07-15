using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.IndustryDTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class UpdateCompanyModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateCompanyModel(IHttpClientFactory httpClientFactory)
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
            if (role != "EMPLOYER")
                return RedirectToPage("/NotFound");

            var token = HttpContext.Session.GetString("jwt_token");
            var userIdStr = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdStr))
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Lấy thông tin công ty của user hiện tại
            var companyResponse = await _httpClient.GetAsync($"api/Companies/user/{userIdStr}");
            if (!companyResponse.IsSuccessStatusCode)
                return RedirectToPage("/NotFound");

            var companyJson = await companyResponse.Content.ReadAsStringAsync();
            Company = JsonSerializer.Deserialize<CompanyCreateUpdateDTO>(companyJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

            // Lấy danh sách ngành nghề
            var industryResponse = await _httpClient.GetAsync("api/Industries");
            if (industryResponse.IsSuccessStatusCode)
            {
                var industryJson = await industryResponse.Content.ReadAsStringAsync();
                Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(industryJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            var userIdStr = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdStr))
                return RedirectToPage("/Common/Login");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(Company);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("api/Companies", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật công ty thành công!";
                return RedirectToPage("/Companies/CompanyInformation");
            }

            TempData["Error"] = "Cập nhật công ty thất bại!";
            return Page();
        }
    }
}
