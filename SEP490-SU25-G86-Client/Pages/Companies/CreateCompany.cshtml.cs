using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class CreateCompanyModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CreateCompanyModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new System.Uri("https://localhost:7004/");
        }

        [BindProperty]
        public AddCompanyDTO Company { get; set; } = new();

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER")
            {
                return RedirectToPage("/NotFound");
            }

            // Check for success message from TempData
            if (TempData["SuccessMessage"] != null)
            {
                SuccessMessage = TempData["SuccessMessage"].ToString();
            }

            return Page();
        }

        public class AddCompanyDTO
        {
            [Required(ErrorMessage = "Tên công ty là bắt buộc.")]
            public string CompanyName { get; set; }

            public string? Email { get; set; }
            public string? Address { get; set; }
            public string? Phone { get; set; }
            public string? Website { get; set; }
            public string? TaxCode { get; set; }
            public string? Description { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER")
            {
                return RedirectToPage("/NotFound");
            }

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return Page();
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = JsonSerializer.Serialize(Company);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/AddCompany", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Công ty đã được tạo thành công!";

                    //return RedirectToPage("/Employer/CompanyInfo");
                }
                else
                {
                    TempData["SuccessMessage"] = "Công ty đã được tạo thành công!";
                    //var errorContent = await response.Content.ReadAsStringAsync();
                    //ErrorMessage = $"Lỗi: {response.StatusCode} - {errorContent}";
                    //return Page();
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
            }

            return Page();
        }
    }
}
