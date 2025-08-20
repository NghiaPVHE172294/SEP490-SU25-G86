using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Text;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class ResendVerificationEmailModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        public string Message { get; set; }

        public void OnGet()
        {
            Message = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Message = string.Empty;
            if (string.IsNullOrEmpty(Email))
            {
                Message = "Vui lòng nhập email.";
                return Page();
            }
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7004");
                    var content = new StringContent($"\"{Email}\"", Encoding.UTF8, "application/json");
                    var resp = await client.PostAsync("/api/Auth/resend-verification-email", content);
                    if (resp.IsSuccessStatusCode)
                    {
                        Message = "Đã gửi lại email xác thực. Vui lòng kiểm tra hộp thư.";
                    }
                    else
                    {
                        var msg = await resp.Content.ReadAsStringAsync();
                        Message = $"Lỗi: {msg}";
                    }
                }
            }
            catch (Exception ex)
            {
                Message = $"Lỗi gửi lại email: {ex.Message}";
            }
            return Page();
        }
    }
}
