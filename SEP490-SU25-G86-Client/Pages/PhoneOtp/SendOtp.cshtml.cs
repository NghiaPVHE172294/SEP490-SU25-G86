using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.PhoneOtp
{
    public class SendOtpModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public SendOtpModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public string PhoneNumber { get; set; }

        public string Message { get; set; }
        [BindProperty(SupportsGet = true)]
        public int UserId { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            if (UserId <= 0)
            {
                Message = "Không tìm thấy thông tin người dùng. Vui lòng đăng nhập lại hoặc cung cấp userId.";
                return Page();
            }

            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                ModelState.AddModelError("PhoneNumber", "Số điện thoại không được để trống.");
                return Page();
            }

            var client = _clientFactory.CreateClient();
            var token = HttpContext.Session.GetString("jwt_token");

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Message = "Không tìm thấy thông tin đăng nhập. Vui lòng đăng nhập lại.";
                return Page();
            }

            var payload = new { phone = PhoneNumber };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7004/api/PhoneOtp/send-otp", content);
            if (response.IsSuccessStatusCode)
            {
                // Gửi cả PhoneNumber và UserId qua TempData để trang Verify sử dụng
                TempData["PhoneNumber"] = PhoneNumber;
                TempData["UserId"] = UserId;
                return RedirectToPage("VerifyOtp");
            }

            Message = "Không thể gửi OTP. Vui lòng thử lại.";
            return Page();
        }

    }
}
