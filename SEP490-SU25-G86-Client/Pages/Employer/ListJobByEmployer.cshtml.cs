using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class ListJobByEmployerModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<JobPostListDTO> Jobs { get; set; }
        public HashSet<int> JobPostIdsWithCriteria { get; set; } = new();

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

            var response = await _httpClient.GetAsync("api/JobPosts/employer");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Jobs = JsonSerializer.Deserialize<List<JobPostListDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                Jobs = new List<JobPostListDTO>();
            }

            // Lấy toàn bộ JobCriteria của user
            var criteriaResponse = await _httpClient.GetAsync("api/jobcriterion/my");
            if (criteriaResponse.IsSuccessStatusCode)
            {
                var criteriaContent = await criteriaResponse.Content.ReadAsStringAsync();
                var jobCriteria = JsonSerializer.Deserialize<List<JobCriterionDTO>>(criteriaContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                JobPostIdsWithCriteria = jobCriteria != null ? jobCriteria.Select(c => c.JobPostId).ToHashSet() : new HashSet<int>();
            }
            else
            {
                JobPostIdsWithCriteria = new HashSet<int>();
            }

            // DEBUG LOG
            System.Diagnostics.Debug.WriteLine($"DEBUG: JobPostIdsWithCriteria = [{string.Join(",", JobPostIdsWithCriteria)}]");
            if (Jobs != null)
            {
                foreach (var job in Jobs)
                {
                    System.Diagnostics.Debug.WriteLine($"DEBUG: JobPostId = {job.JobPostId}, HasCriteria = {JobPostIdsWithCriteria.Contains(job.JobPostId)}");
                }
            }

            return Page();
        }


    }
}