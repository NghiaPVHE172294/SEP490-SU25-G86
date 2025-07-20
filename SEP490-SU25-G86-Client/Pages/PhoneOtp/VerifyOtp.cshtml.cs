using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.PhoneOtp
{
    public class VerifyOtpModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public VerifyOtpModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public string OTP { get; set; }

        [BindProperty]
        public string PhoneNumber { get; set; }

        [BindProperty]
        public int UserId { get; set; } = 123; // giả sử đã xác định user đang đăng nhập

        public string Message { get; set; }

        public void OnGet()
        {
            PhoneNumber = TempData["PhoneNumber"]?.ToString();
            if (TempData["UserId"] != null && int.TryParse(TempData["UserId"].ToString(), out int id))
            {
                UserId = id;
            }
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(OTP) || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ thông tin.");
                return Page();
            }

            var client = _clientFactory.CreateClient();

            // ✅ Gắn JWT vào header nếu có
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

            var payload = new
            {
                userId = UserId,
                phone = PhoneNumber,
                otp = OTP
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7004/api/PhoneOtp/verify-otp",content);

            if (response.IsSuccessStatusCode)
            {
                Message = "Xác thực thành công!";
            }
            else
            {
                Message = "OTP không hợp lệ hoặc đã hết hạn.";
            }

            return Page();
        }
    }
}
