using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class registerModel : PageModel
    {
        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }
        [BindProperty]
        public bool AcceptTerms { get; set; }
        [BindProperty]
        public string RoleName { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!AcceptTerms)
            {
                ErrorMessage = "Bạn phải đồng ý với điều khoản dịch vụ.";
                return Page();
            }
            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return Page();
            }
            if (!IsValidPassword(Password))
            {
                ErrorMessage = "Mật khẩu mới phải có ít nhất 1 chữ hoa, 1 chữ thường, 1 số và tối thiểu 6 ký tự.";
                return Page();
            }
            // Kiểm tra email trùng bằng gọi API
            var client = new HttpClient();
            var checkEmailResp = await client.GetAsync($"https://localhost:7004/api/Auth/check-email?email={Email}");
            if (checkEmailResp.IsSuccessStatusCode)
            {
                var exists = bool.Parse(await checkEmailResp.Content.ReadAsStringAsync());
                if (exists)
                {
                    ErrorMessage = "Email đã tồn tại.";
                    return Page();
                }
            }
            // Không mã hóa password ở frontend nữa
            var registerData = new { FullName, Email, Password, RoleName };
            var content = new StringContent(JsonSerializer.Serialize(registerData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7004/api/Auth/register", content);
            if (response.IsSuccessStatusCode)
            {
                // Đăng ký thành công, lưu thông báo vào TempData và chuyển về Login
                TempData["RegisterSuccess"] = "Đăng ký thành công! Vui lòng kiểm tra email để xác thực tài khoản.";
                return RedirectToPage("/Common/Login");
            }
            else
            {
                var resp = await response.Content.ReadAsStringAsync();
                ErrorMessage = !string.IsNullOrEmpty(resp) ? resp : "Đăng ký thất bại.";
                return Page();
            }
        }

        private bool IsValidPassword(string password)
        {
            // Có ít nhất 1 chữ hoa, 1 chữ thường, 1 số, tối thiểu 6 ký tự
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$");
        }
    }
}
