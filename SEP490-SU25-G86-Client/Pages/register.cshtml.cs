using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;

namespace SEP490_SU25_G86_Client.Pages
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
                ErrorMessage = "Mật khẩu phải có chữ hoa, chữ thường và số.";
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
            // Mã hóa password bằng MD5
            string hashedPassword = GetMd5Hash(Password);
            var registerData = new { FullName, Email, Password = hashedPassword, RoleName };
            var content = new StringContent(JsonSerializer.Serialize(registerData), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7004/api/Auth/register", content);
            if (response.IsSuccessStatusCode)
            {
                // Đăng ký thành công, chuyển về Login
                return RedirectToPage("/Login");
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
        private string GetMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
