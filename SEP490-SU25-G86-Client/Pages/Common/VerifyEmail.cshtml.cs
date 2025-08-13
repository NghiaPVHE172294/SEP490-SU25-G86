using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class VerifyEmailModel : PageModel
    {
        public string Status { get; set; } = "Đang xác thực...";
        public bool IsSuccess { get; set; } = false;

        public async Task OnGet()
        {
            var token = Request.Query["token"].ToString();
            if (string.IsNullOrEmpty(token))
            {
                Status = "Thiếu mã xác thực.";
                return;
            }
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7004");
                    var resp = await client.GetAsync($"/api/Auth/verify-email?token={token}");
                    if (resp.IsSuccessStatusCode)
                    {
                        Status = "Xác thực thành công! Bạn có thể đăng nhập.";
                        IsSuccess = true;
                    }
                    else
                    {
                        var msg = await resp.Content.ReadAsStringAsync();
                        Status = $"Xác thực thất bại: {msg}";
                    }
                }
            }
            catch (Exception ex)
            {
                Status = $"Lỗi xác thực: {ex.Message}";
            }
        }
    }
}
