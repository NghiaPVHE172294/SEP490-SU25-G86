using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class ChangePasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        [BindProperty]
        public ChangePasswordDTO ChangePassword { get; set; } = new();

        public string? ToastMessage { get; set; }
        public string ToastColor { get; set; } = "bg-success";

        public ChangePasswordModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGetAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                Response.Redirect("/Common/Login");
                return;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Common/Login");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(ChangePassword), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7004/api/Auth/change-password", content);

            if (response.IsSuccessStatusCode)
            {
                ToastMessage = "✅ Đổi mật khẩu thành công.";
                ToastColor = "bg-success";
            }
            else
            {
                var res = await response.Content.ReadAsStringAsync();
                try
                {
                    var json = JsonDocument.Parse(res);
                    var msg = json.RootElement.GetProperty("message").GetString();
                    ToastMessage = $"❌ {msg}";
                }
                catch
                {
                    ToastMessage = "❌ Đổi mật khẩu thất bại.";
                }
                ToastColor = "bg-danger";
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
