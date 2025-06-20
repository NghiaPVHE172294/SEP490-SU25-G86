using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class ResetPasswordModel : PageModel
    {
        [BindProperty]
        public string Token { get; set; }
        [BindProperty]
        public string NewPassword { get; set; }
        [BindProperty]
        public string ConfirmPassword { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            Token = Request.Query["token"].ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return Page();
            }
            if (NewPassword != ConfirmPassword)
            {
                ErrorMessage = "Mật khẩu xác nhận không khớp.";
                return Page();
            }
            if (NewPassword.Length < 6)
            {
                ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.";
                return Page();
            }
            var client = new HttpClient();
            var payload = new { Token, NewPassword };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7004/api/Auth/reset-password", content);
            if (response.IsSuccessStatusCode)
            {
                Message = "Đặt lại mật khẩu thành công. Bạn có thể đăng nhập với mật khẩu mới.";
            }
            else
            {
                ErrorMessage = await response.Content.ReadAsStringAsync();
            }
            return Page();
        }
    }
} 