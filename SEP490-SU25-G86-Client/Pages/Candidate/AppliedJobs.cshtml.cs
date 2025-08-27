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
        public List<SEP490_SU25_G86_API.vn.edu.fpt.DTO.SavedJobDTO.SavedJobDTO> SavedJobs { get; set; } = new();
        private readonly HttpClient _httpClient;
        public List<AppliedJobDTO> AppliedJobs { get; set; } = new();
        public List<JobPostHomeDto> SuggestedJobs { get; set; } = new();

        // Pagination properties
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }
        public List<AppliedJobDTO> PagedJobs { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public Dictionary<string, string> StatusMap { get; } = new Dictionary<string, string>
        {
            { "Đã ứng tuyển", "Đã ứng tuyển" },
            {"Đã chấm điểm bằng AI", "Hồ sơ đang được xử lý"},
            { "ĐÃ DUYỆT", "Hồ sơ phù hợp" },
            { "ĐÃ TỪ CHỐI", "Hồ sơ không phù hợp" },
            { "Hồ sơ đã rút", "Hồ sơ đã rút" }
        };

        public AppliedJobsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy userId và token 1 lần duy nhất
            var userIdStr = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
            {
                return RedirectToPage("/Common/Login");
            }
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Lấy danh sách việc làm đã lưu
            var savedJobsResp = await _httpClient.GetAsync($"api/SavedJobs/user/{userId}");
            if (savedJobsResp.IsSuccessStatusCode)
            {
                var content = await savedJobsResp.Content.ReadAsStringAsync();
                SavedJobs = JsonSerializer.Deserialize<List<SEP490_SU25_G86_API.vn.edu.fpt.DTO.SavedJobDTO.SavedJobDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else
            {
                SavedJobs = new();
            }

            var role = HttpContext.Session.GetString("user_role");
            if (role != "CANDIDATE")
            {
                return RedirectToPage("/NotFound");
            }


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
            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "Trạng thái" && StatusFilter != "" && StatusFilter != "Tất cả trạng thái")
            {
                string backendStatus = StatusFilter;

                if (StatusFilter == "Hồ sơ đã rút")
                {
                    AppliedJobs = AppliedJobs.Where(j => (j.Status ?? "").Equals("Hồ sơ đã rút", StringComparison.OrdinalIgnoreCase) && j.IsDelete == true).ToList();
                }
                else
                {
                    AppliedJobs = AppliedJobs.Where(j => (j.IsDelete == false || j.IsDelete == null) && (j.Status ?? "").Equals(backendStatus, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }
            // Pagination logic
            if (int.TryParse(HttpContext.Request.Query["page"], out int page))
                Page = page > 0 ? page : 1;
            TotalPages = (int)Math.Ceiling(AppliedJobs.Count / (double)PageSize);
            PagedJobs = AppliedJobs.Skip((Page - 1) * PageSize).Take(PageSize).ToList();

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