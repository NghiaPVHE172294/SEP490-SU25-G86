using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AccountDTO;
using System.ComponentModel.DataAnnotations;
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
        [BindProperty]
        public UserProfileUpdateDTO UserUpdate { get; set; } = new();

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
            // Load lại profile để giữ dữ liệu từ DB khi cập nhật fail
            if (!string.IsNullOrEmpty(UserUpdate.FullName) && UserUpdate.FullName.Length > 30)
            {
                var clientRefresh = _httpClientFactory.CreateClient();
                clientRefresh.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var getRes = await clientRefresh.GetAsync("https://localhost:7004/api/user/profile");
                if (getRes.IsSuccessStatusCode)
                {
                    var json = await getRes.Content.ReadAsStringAsync();
                    var profile = JsonSerializer.Deserialize<UserProfileDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (profile != null)
                        UserProfile = profile;
                }

                ToastMessage = "❌ Tên không được vượt quá 30 ký tự.";
                ToastColor = "bg-danger";
                return Page();
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // ✅ Gửi multipart/form-data
            var formData = new MultipartFormDataContent();

            if (!string.IsNullOrWhiteSpace(UserUpdate.FullName))
                formData.Add(new StringContent(UserUpdate.FullName), "FullName");
            if (!string.IsNullOrWhiteSpace(UserUpdate.Address))
                formData.Add(new StringContent(UserUpdate.Address), "Address");
            if (!string.IsNullOrWhiteSpace(UserUpdate.Phone))
                formData.Add(new StringContent(UserUpdate.Phone), "Phone");
            if (!string.IsNullOrWhiteSpace(UserUpdate.Dob))
                formData.Add(new StringContent(UserUpdate.Dob), "Dob");
            if (!string.IsNullOrWhiteSpace(UserUpdate.LinkedIn))
                formData.Add(new StringContent(UserUpdate.LinkedIn), "LinkedIn");
            if (!string.IsNullOrWhiteSpace(UserUpdate.Facebook))
                formData.Add(new StringContent(UserUpdate.Facebook), "Facebook");
            if (!string.IsNullOrWhiteSpace(UserUpdate.AboutMe))
                formData.Add(new StringContent(UserUpdate.AboutMe), "AboutMe");

            if (UserUpdate.AvatarFile != null && UserUpdate.AvatarFile.Length > 0)
            {
                if (!UserUpdate.AvatarFile.ContentType.StartsWith("image/"))
                {
                    ToastMessage = "❌ File không hợp lệ. Vui lòng chọn ảnh.";
                    ToastColor = "bg-danger";
                    return RedirectToPage();
                }

                var streamContent = new StreamContent(UserUpdate.AvatarFile.OpenReadStream());
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(UserUpdate.AvatarFile.ContentType);
                formData.Add(streamContent, "AvatarFile", UserUpdate.AvatarFile.FileName);
            }

            var response = await client.PutAsync("https://localhost:7004/api/user/profile", formData);

            if (response.IsSuccessStatusCode)
            {
                ToastMessage = "✅ Cập nhật thông tin thành công.";
                ToastColor = "bg-success";

                // Refresh thông tin
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
                Console.WriteLine("Lỗi API upload avatar: " + msg); // Log ra console server hoặc local dev

                try
                {
                    var json = JsonDocument.Parse(msg);
                    if (json.RootElement.TryGetProperty("message", out var messageProperty))
                    {
                        ToastMessage = $"❌ {messageProperty.GetString()}";
                    }
                    else
                    {
                        ToastMessage = $"❌ Lỗi không rõ: {msg}";
                    }
                }
                catch
                {
                    ToastMessage = $"❌ Lỗi không thể phân tích chi tiết: {msg}";
                }
            }

            return Page();
        }
    }
    public class UserProfileDTO
    {
        public int Id { get; set; }
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
    public class UserProfileUpdateDTO
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Dob { get; set; }
        public string? LinkedIn { get; set; }
        public string? Facebook { get; set; }
        public string? AboutMe { get; set; }

        public IFormFile? AvatarFile { get; set; } 
    }
}
