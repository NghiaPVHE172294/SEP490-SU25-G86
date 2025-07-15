using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using System.Linq;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class ListJobCriteriaModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ListJobCriteriaModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<JobCriterionDTO> JobCriteria { get; set; } = new();
        public List<JobPostListDTO> JobPosts { get; set; } = new();
        public Dictionary<int, List<JobCriterionDTO>> CriteriaByJobPost { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER")
            {
                return RedirectToPage("/NotFound");
            }
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToPage("/Common/Login");
            }
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri("https://localhost:7004/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Lấy danh sách JobPost
            var jobsResponse = await client.GetAsync("api/jobposts/employer");
            if (jobsResponse.IsSuccessStatusCode)
            {
                var jobsJson = await jobsResponse.Content.ReadAsStringAsync();
                JobPosts = JsonSerializer.Deserialize<List<JobPostListDTO>>(jobsJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            // Lấy danh sách JobCriteria
            var response = await client.GetAsync("api/jobcriterion/my");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                JobCriteria = JsonSerializer.Deserialize<List<JobCriterionDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToPage("/Common/Login");
            }
            // Group JobCriteria theo JobPostId
            CriteriaByJobPost = JobCriteria.GroupBy(jc => jc.JobPostId)
                .ToDictionary(g => g.Key, g => g.ToList());
            return Page();
        }

    }
} 