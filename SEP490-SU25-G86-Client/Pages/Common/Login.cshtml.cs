using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return Page();
            }

            var client = new HttpClient();
            var loginData = new { Email, Password };
            var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");
            try
            {
                var response = await client.PostAsync("https://localhost:7004/api/Auth/login", content);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var json = JsonDocument.Parse(responseBody).RootElement;
                    var token = json.GetProperty("token").GetString();
                    var role = json.GetProperty("role").GetString();
                    var userId = json.GetProperty("userId").GetInt32();
                    // Lưu token vào session
                    HttpContext.Session.SetString("jwt_token", token);
                    HttpContext.Session.SetString("user_role", role);
                    HttpContext.Session.SetString("userId", userId.ToString());
                    HttpContext.Session.SetInt32("user_id", userId);
                    // Chuyển hướng theo role
                    if (role == "ADMIN") return RedirectToPage("/Admin/AdminDashboard");
                    //if (role == "EMPLOYER") return RedirectToPage("/EmployerDashboard");
                    // Candidate và Employer đều về Homepage
                    return RedirectToPage("/Common/Homepage", new { token = token });
                }
                else
                {
                    var json = JsonDocument.Parse(responseBody).RootElement;
                    ErrorMessage = json.TryGetProperty("message", out var msg) ? msg.GetString() : "Đăng nhập thất bại.";
                    return Page();
                }
            }
            catch
            {
                ErrorMessage = "Không thể kết nối tới máy chủ.";
                return Page();
            }
        }
    }
}
