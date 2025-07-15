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
        public UserProfileDTO UserProfile { get; set; } = new();

        public string? ToastMessage { get; set; }
        public string ToastColor { get; set; } = "bg-info";

        public SettingProfileModel(IHttpClientFactory httpClientFactory)
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

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync("https://localhost:7004/api/user/profile");

            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var profile = JsonSerializer.Deserialize<UserProfileDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (profile != null)
                    UserProfile = profile;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Common/Login");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(UserProfile), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("https://localhost:7004/api/user/profile", content);

            if (response.IsSuccessStatusCode)
            {
                ToastMessage = "✅ Cập nhật thông tin thành công.";
                ToastColor = "bg-success";

                var getRes = await client.GetAsync("https://localhost:7004/api/user/profile");
                if (getRes.IsSuccessStatusCode)
                {
                    var json = await getRes.Content.ReadAsStringAsync();
                    var profile = JsonSerializer.Deserialize<UserProfileDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (profile != null)
                        UserProfile = profile;
                }
            }
            else
            {
                var msg = await response.Content.ReadAsStringAsync();
                try
                {
                    var json = JsonDocument.Parse(msg);
                    ToastMessage = $"❌ {json.RootElement.GetProperty("message").GetString()}";
                }
                catch
                {
                    ToastMessage = "❌ Cập nhật thất bại.";
                }
                ToastColor = "bg-danger";
            }

            return Page();
        }
    }
    public class UserProfileDTO
    {
        public string? Avatar { get; set; }
        public string FullName { get; set; } = null!;
        public string? Address { get; set; }
        public string? Email { get; set; } 
        public string? Phone { get; set; }
        public string? Dob { get; set; } 
        public string? LinkedIn { get; set; }
        public string? Facebook { get; set; }
        public string? AboutMe { get; set; }
    }
}
