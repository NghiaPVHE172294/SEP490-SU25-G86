using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.SavedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;

namespace SEP490_SU25_G86_Client.Pages.SavedJobs
{
    public class SavedJobsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<SavedJobDTO> SavedJobs { get; set; } = new();
        public List<JobPostHomeDto> SuggestedJobs { get; set; } = new();

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

            var userIdStr = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Common/Login");
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

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

            // Lấy 10 job mới nhất cho gợi ý
            var suggestResponse = await _httpClient.GetAsync($"api/jobposts/homepage?page=1&pageSize=10");
            if (suggestResponse.IsSuccessStatusCode)
            {
                var suggestContent = await suggestResponse.Content.ReadAsStringAsync();
                var suggestResult = JsonSerializer.Deserialize<SuggestedJobApiResponse>(suggestContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                SuggestedJobs = suggestResult?.Jobs ?? new List<JobPostHomeDto>();
            }
            else
            {
                SuggestedJobs = new List<JobPostHomeDto>();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int saveJobId)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"api/SavedJobs/{saveJobId}");
            // Sau khi xóa, reload lại trang
            return RedirectToPage();
        }

        private class SuggestedJobApiResponse
        {
            public List<JobPostHomeDto> Jobs { get; set; } = new();
        }
    }
}
