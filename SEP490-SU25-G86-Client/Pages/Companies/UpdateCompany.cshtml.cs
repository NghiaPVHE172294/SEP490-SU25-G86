using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.IndustryDTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class UpdateCompanyModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateCompanyModel(IHttpClientFactory httpClientFactory)
        {
            // Tạo HttpClient dùng để gọi API
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        // Dữ liệu bind từ form gửi về
        [BindProperty]
        public CompanyCreateUpdateDTO Company { get; set; } = new();

        // Danh sách ngành nghề lấy từ API để render dropdown
        public List<IndustryDTO> Industries { get; set; } = new();

        // Danh sách ngành nghề dạng SelectList để bind vào <select>
        public List<SelectListItem> IndustrySelectList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra quyền truy cập
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER") return RedirectToPage("/NotFound");

            // Lấy token từ session
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Common/Login");

            // Thêm token vào header khi gọi API
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API để lấy thông tin công ty của người dùng hiện tại (không cần userId)
            var companyRes = await _httpClient.GetAsync("api/Companies/me");
            if (!companyRes.IsSuccessStatusCode) return RedirectToPage("/NotFound");

            // Deserialize dữ liệu công ty
            var json = await companyRes.Content.ReadAsStringAsync();
            var companyDetail = JsonSerializer.Deserialize<CompanyDetailDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (companyDetail == null) return RedirectToPage("/NotFound");

            // Gán thông tin sang DTO để bind vào form
            Company = new CompanyCreateUpdateDTO
            {
                CompanyName = companyDetail.CompanyName,
                TaxCode = companyDetail.TaxCode,
                IndustryId = companyDetail.IndustryId,
                Email = companyDetail.Email,
                Phone = companyDetail.Phone,
                Address = companyDetail.Address,
                Description = companyDetail.Description,
                Website = companyDetail.Website,
                CompanySize = companyDetail.CompanySize,
                LogoUrl = companyDetail.LogoUrl
            };

            // Gọi API để lấy danh sách ngành nghề
            var industryResponse = await _httpClient.GetAsync("api/Industries");
            if (industryResponse.IsSuccessStatusCode)
            {
                var industryJson = await industryResponse.Content.ReadAsStringAsync();
                Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(industryJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                // Tạo danh sách select option, đánh dấu selected nếu trùng IndustryId hiện tại
                IndustrySelectList = Industries.Select(i => new SelectListItem
                {
                    Text = i.IndustryName,
                    Value = i.IndustryId.ToString(),
                    Selected = (i.IndustryId == Company.IndustryId)
                }).ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Kiểm tra xác thực
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Common/Login");

            // Gán token
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Lấy lại thông tin công ty hiện tại từ API (để lấy CompanyId)
            var companyInfoRes = await _httpClient.GetAsync("api/Companies/me");
            if (!companyInfoRes.IsSuccessStatusCode) return RedirectToPage("/NotFound");

            var jsonCompany = await companyInfoRes.Content.ReadAsStringAsync();
            var companyDetail = JsonSerializer.Deserialize<CompanyDetailDTO>(jsonCompany, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (companyDetail == null) return RedirectToPage("/NotFound");

            // Gửi PUT request để cập nhật công ty
            var json = JsonSerializer.Serialize(Company);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var updateResponse = await _httpClient.PutAsync($"api/Companies/{companyDetail.CompanyId}", content);
            if (updateResponse.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật công ty thành công.";
                return RedirectToPage("/Companies/CompanyInformation");
            }

            TempData["Error"] = "Cập nhật công ty thất bại.";
            return Page();
        }
    }
}
