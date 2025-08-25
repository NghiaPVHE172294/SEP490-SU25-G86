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

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }
        public List<SavedJobDTO> PagedJobs { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public Dictionary<string, string> StatusMap { get; } = new Dictionary<string, string>
        {
            { "OPEN", "Đang mở" },
            { "CLOSED", "Hết hiệu lực" }
        };

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
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Lấy danh sách việc làm đã lưu
            var response = await _httpClient.GetAsync($"api/SavedJobs/user/{userId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                SavedJobs = JsonSerializer.Deserialize<List<SavedJobDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                SavedJobs = new List<SavedJobDTO>();
            }

            // Filter theo trạng thái nếu có
            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "Trạng thái" && StatusFilter != "Tất cả trạng thái")
            {
                SavedJobs = SavedJobs.Where(j => (j.Status ?? "").Equals(StatusFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Pagination
            if (int.TryParse(HttpContext.Request.Query["page"], out int page))
                Page = page > 0 ? page : 1;

            TotalPages = (int)Math.Ceiling(SavedJobs.Count / (double)PageSize);
            PagedJobs = SavedJobs.Skip((Page - 1) * PageSize).Take(PageSize).ToList();

            // Gợi ý việc làm
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

            await _httpClient.DeleteAsync($"api/SavedJobs/{saveJobId}");

            return RedirectToPage();
        }

        private class SuggestedJobApiResponse
        {
            public List<JobPostHomeDto> Jobs { get; set; } = new();
        }
    }
}
