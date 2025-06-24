using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class AddJobPostModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public AddJobPostModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new System.Uri("https://localhost:7004/");
        }

        [BindProperty]
        public AddJobPostDTO JobPost { get; set; } = new();

        public string ErrorMessage { get; set; }

        public List<EmploymentType> EmploymentTypes { get; set; } = new();
        public List<JobPosition> JobPositions { get; set; } = new();
        public List<Province> Provinces { get; set; } = new();
        public List<ExperienceLevel> ExperienceLevels { get; set; } = new();
        public List<JobLevel> JobLevels { get; set; } = new();
        public List<Industry> Industries { get; set; } = new();

        public class EmploymentType
        {
            public int EmploymentTypeId { get; set; }
            public string EmploymentTypeName { get; set; }
        }
        public class JobPosition
        {
            public int PositionId { get; set; }
            public string PostitionName { get; set; }
        }
        public class Province
        {
            public int ProvinceId { get; set; }
            public string ProvinceName { get; set; }
        }
        public class ExperienceLevel
        {
            public int ExperienceLevelId { get; set; }
            public string ExperienceLevelName { get; set; }
        }
        public class JobLevel
        {
            public int JobLevelId { get; set; }
            public string JobLevelName { get; set; }
        }
        public class Industry
        {
            public int IndustryId { get; set; }
            public string IndustryName { get; set; }
        }

        public async Task OnGetAsync()
        {
            EmploymentTypes = await _httpClient.GetFromJsonAsync<List<EmploymentType>>("api/employmenttypes");
            JobPositions = await _httpClient.GetFromJsonAsync<List<JobPosition>>("api/jobpositions");
            Provinces = await _httpClient.GetFromJsonAsync<List<Province>>("api/provinces");
            ExperienceLevels = await _httpClient.GetFromJsonAsync<List<ExperienceLevel>>("api/experiencelevels");
            JobLevels = await _httpClient.GetFromJsonAsync<List<JobLevel>>("api/joblevels");
            Industries = await _httpClient.GetFromJsonAsync<List<Industry>>("api/industries");
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                return Page();
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(JobPost), Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PostAsync("api/jobposts", content);
                if (response.IsSuccessStatusCode)
                {
                    var resp = await response.Content.ReadAsStringAsync();
                    var job = JsonDocument.Parse(resp).RootElement;
                    int jobPostId = job.GetProperty("jobPostId").GetInt32();
                    return Redirect($"/Job/DetailJobPost/{jobPostId}");
                }
                else
                {
                    ErrorMessage = $"Lỗi: {response.StatusCode}";
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
            }
            return Page();
        }
    }
} 