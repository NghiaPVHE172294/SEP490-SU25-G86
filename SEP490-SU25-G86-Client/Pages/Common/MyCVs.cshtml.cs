using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class MyCVsModel : PageModel
    {
        public List<CvDTO> CVs { get; set; } = new();

        [BindProperty]
        public string CVName { get; set; }
        [BindProperty]
        public string? Notes { get; set; }
        [BindProperty]
        public IFormFile? File { get; set; }
        [BindProperty]
        public int CvId { get; set; }

        private const string ApiBase = "https://localhost:7004/";

        public async Task OnGetAsync()
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(ApiBase + "api/Cv/my");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                CVs = JsonSerializer.Deserialize<List<CvDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (File == null || string.IsNullOrEmpty(CVName))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn file và nhập tên CV.");
                await OnGetAsync();
                return Page();
            }
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var content = new MultipartFormDataContent();
            content.Add(new StreamContent(File.OpenReadStream()), "File", File.FileName);
            content.Add(new StringContent(CVName), "CVName");
            if (!string.IsNullOrEmpty(Notes))
                content.Add(new StringContent(Notes), "Notes");
            var response = await client.PostAsync(ApiBase + "api/Cv/upload", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, "Tải lên thất bại: " + errorMsg);
                await OnGetAsync();
                return Page();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (CvId == 0 || string.IsNullOrEmpty(CVName))
            {
                ModelState.AddModelError(string.Empty, "Thiếu thông tin cập nhật.");
                await OnGetAsync();
                return Page();
            }
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PutAsync(ApiBase + $"api/Cv/rename/{CvId}", new StringContent($"\"{CVName}\"", Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Cập nhật thất bại: " + await response.Content.ReadAsStringAsync());
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            if (CvId == 0)
            {
                ModelState.AddModelError(string.Empty, "Thiếu thông tin xóa.");
                await OnGetAsync();
                return Page();
            }
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.DeleteAsync(ApiBase + $"api/Cv/{CvId}");
            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Xóa thất bại: " + await response.Content.ReadAsStringAsync());
            }
            return RedirectToPage();
        }
    }
}
