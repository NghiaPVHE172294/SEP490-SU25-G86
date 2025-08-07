using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class ListJobByEmployerModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<JobPostListDTO> Jobs { get; set; } = new();
        public HashSet<int> JobPostIdsWithCriteria { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public ListJobByEmployerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER")
            {
                return RedirectToPage("/NotFound");
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Lấy toàn bộ JobPost
            var response = await _httpClient.GetAsync("api/JobPosts/employer");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Jobs = JsonSerializer.Deserialize<List<JobPostListDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                // Lọc theo trạng thái nếu có
                if (!string.IsNullOrEmpty(StatusFilter))
                {
                    Jobs = Jobs
                        .Where(j => j.Status != null && j.Status.Equals(StatusFilter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            // Lấy toàn bộ JobCriteria
            var criteriaResponse = await _httpClient.GetAsync("api/jobcriterion/my");
            if (criteriaResponse.IsSuccessStatusCode)
            {
                var criteriaContent = await criteriaResponse.Content.ReadAsStringAsync();
                var jobCriteria = JsonSerializer.Deserialize<List<JobCriterionDTO>>(criteriaContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                JobPostIdsWithCriteria = jobCriteria != null ? jobCriteria.Select(c => c.JobPostId).ToHashSet() : new();
            }

            return Page();
        }
    }
}
