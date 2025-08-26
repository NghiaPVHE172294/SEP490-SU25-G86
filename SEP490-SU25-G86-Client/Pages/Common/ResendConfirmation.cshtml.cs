using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class ResendConfirmationModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; }
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Vui lòng nhập email.";
                return Page();
            }
            var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(Email), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7004/api/Auth/resend-verification-email", content);
            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Đã gửi lại email xác nhận. Vui lòng kiểm tra hộp thư đến.";
                ErrorMessage = null;
            }
            else
            {
                var resp = await response.Content.ReadAsStringAsync();
                ErrorMessage = !string.IsNullOrEmpty(resp) ? resp : "Gửi lại email xác nhận thất bại.";
                SuccessMessage = null;
            }
            return Page();
        }
    }
}
