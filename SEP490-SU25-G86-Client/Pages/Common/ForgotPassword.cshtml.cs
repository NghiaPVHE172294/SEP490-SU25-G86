using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class ForgotPasswordModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                ErrorMessage = "Vui lòng nhập email.";
                return Page();
            }
            var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(Email), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7004/api/Auth/forgot-password", content);
            if (response.IsSuccessStatusCode)
            {
                Message = "Nếu email tồn tại, hệ thống đã gửi hướng dẫn đặt lại mật khẩu.";
            }
            else
            {
                ErrorMessage = await response.Content.ReadAsStringAsync();
            }
            return Page();
        }
    }
}
