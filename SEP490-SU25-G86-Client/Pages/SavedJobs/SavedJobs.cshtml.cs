using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.SavedJobDTO;

namespace SEP490_SU25_G86_Client.Pages.SavedJobs
{
    public class SavedJobsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<SavedJobDTO> SavedJobs { get; set; } = new();

        public SavedJobsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "CANDIDATE")
            {
                return RedirectToPage("/NotFound");
            }

            // DEMO: Bỏ qua kiểm tra đăng nhập và token
            int userId = 1;

            // Không cần gán Authorization nếu không dùng JWT
            // _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/SavedJobs/user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                SavedJobs = JsonSerializer.Deserialize<List<SavedJobDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                SavedJobs = new List<SavedJobDTO>();
            }

            return Page();
        }
    }
}
