using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AccountDTO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class SettingProfileModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        [BindProperty]
        public ChangePasswordDTO ChangePassword { get; set; } = new();
        public string? ResultMessage { get; set; }
        public SettingProfileModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public void OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("jwt_token")))
            {
                Response.Redirect("/Common/Login");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Common/Login"); // An toàn thêm
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(ChangePassword), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("https://localhost:7004/api/auth/change-password", content);

            if (response.IsSuccessStatusCode)
            {
                ResultMessage = "✅ Đổi mật khẩu thành công.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                try
                {
                    var json = JsonDocument.Parse(error);
                    var msg = json.RootElement.GetProperty("message").GetString();
                    ResultMessage = $"❌ {msg}";
                }
                catch
                {
                    ResultMessage = "❌ Đổi mật khẩu thất bại.";
                }
            }

            return Page();
        }
    }
    public class ChangePasswordDTO
    {
        public string CurrentPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
    }
}
