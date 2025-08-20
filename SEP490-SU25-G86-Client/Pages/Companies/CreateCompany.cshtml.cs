using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.IndustryDTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.IO;

namespace SEP490_SU25_G86_Client.Pages.Companies
{
    public class CreateCompanyModel : PageModel
    {
        private readonly HttpClient _httpClient;

        // Constructor nhận IHttpClientFactory để tạo HttpClient dùng cho gọi API
        public CreateCompanyModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        // Bind dữ liệu từ form tạo công ty
        [BindProperty]
        public CompanyCreateUpdateDTO Company { get; set; } = new();

        // Danh sách ngành nghề (dropdown)
        public List<IndustryDTO> Industries { get; set; } = new();

        // Khi truy cập GET vào trang
        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra vai trò: chỉ EMPLOYER mới được phép tạo công ty
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER") return RedirectToPage("/Common/Login");

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Gọi API lấy danh sách ngành nghề
            var res = await _httpClient.GetAsync("api/Industries");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return Page();
        }

        // Khi submit form POST để tạo công ty
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

            if (!ModelState.IsValid)
            {
                await LoadIndustriesAsync();
                TempData["Error"] = "Thông tin nhập vào không hợp lệ.";
                return Page();
            }

            // Validate logo file is an image
            if (Company.LogoFile != null && Company.LogoFile.Length > 0)
            {
                var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/svg+xml" };
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".svg" };
                var extension = Path.GetExtension(Company.LogoFile.FileName)?.ToLowerInvariant();
                if (!allowedContentTypes.Contains(Company.LogoFile.ContentType) || !allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("Company.LogoFile", "Chỉ cho phép tải ảnh (JPG, PNG, SVG). ");
                    await LoadIndustriesAsync();
                    return Page();
                }
            }

            // Tạo form data multipart
            var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(Company.CompanyName), "CompanyName");
            formData.Add(new StringContent(Company.TaxCode), "TaxCode");
            formData.Add(new StringContent(Company.IndustryId.ToString()), "IndustryId");
            formData.Add(new StringContent(Company.Email), "Email");
            formData.Add(new StringContent(Company.Phone), "Phone");
            formData.Add(new StringContent(Company.Address), "Address");
            formData.Add(new StringContent(Company.Description ?? ""), "Description");
            formData.Add(new StringContent(Company.Website ?? ""), "Website");
            formData.Add(new StringContent(Company.CompanySize), "CompanySize");

            if (Company.LogoFile != null)
            {
                var fileContent = new StreamContent(Company.LogoFile.OpenReadStream());
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(Company.LogoFile.ContentType);
                formData.Add(fileContent, "LogoFile", Company.LogoFile.FileName);
            }

            var response = await _httpClient.PostAsync("api/Companies", formData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Companies/CompanyInformation");
            }
            else
            {
                await LoadIndustriesAsync();
                var errMsg = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Tạo công ty thất bại: {errMsg}";
                return Page();
            }
        }


        // Hàm hỗ trợ load lại danh sách ngành nghề nếu xảy ra lỗi
        private async Task LoadIndustriesAsync()
        {
            var res = await _httpClient.GetAsync("api/Industries");
            if (res.IsSuccessStatusCode)
            {
                var content = await res.Content.ReadAsStringAsync();
                Industries = JsonSerializer.Deserialize<List<IndustryDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }
    }
}
