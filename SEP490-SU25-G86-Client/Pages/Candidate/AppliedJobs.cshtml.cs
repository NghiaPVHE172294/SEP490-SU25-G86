using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AppliedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;

namespace SEP490_SU25_G86_Client.Pages.AppliedJobs
{
    public class AppliedJobsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<AppliedJobDTO> AppliedJobs { get; set; } = new();
        public List<JobPostHomeDto> SuggestedJobs { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public AppliedJobsModel(IHttpClientFactory httpClientFactory)
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

            var response = await _httpClient.GetAsync($"api/AppliedJobs/user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                AppliedJobs = JsonSerializer.Deserialize<List<AppliedJobDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AppliedJobDTO>();
            }
            else
            {
                AppliedJobs = new List<AppliedJobDTO>();
            }

            // Lọc theo trạng thái nếu có
            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "Trạng thái")
            {
                string backendStatus = StatusFilter;
                if (StatusFilter == "Đã ứng tuyển")
                    backendStatus = "OPEN";
                AppliedJobs = AppliedJobs.Where(j => (j.Status ?? "").Equals(backendStatus, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Gợi ý việc làm (kiểu phổ thông)
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

        private class SuggestedJobApiResponse
        {
            public List<JobPostHomeDto> Jobs { get; set; } = new();
        }
    }
} 