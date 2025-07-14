using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class UpdateJobCriterionModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public UpdateJobCriterionModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public UpdateJobCriterionDTO Input { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/NotFound");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri("https://localhost:7004/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"api/jobcriterion/my");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<JobCriterionDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                var item = list.FirstOrDefault(x => x.JobCriteriaId == id);
                if (item == null)
                {
                    ErrorMessage = "Không tìm thấy tiêu chí tuyển dụng.";
                    return Page();
                }
                Input.JobCriteriaId = item.JobCriteriaId;
                Input.JobPostId = item.JobPostId;
                Input.RequiredExperience = item.RequiredExperience;
                Input.RequiredSkills = item.RequiredSkills;
                Input.EducationLevel = item.EducationLevel;
                Input.RequiredJobTitles = item.RequiredJobTitles;
                Input.PreferredLanguages = item.PreferredLanguages;
                Input.PreferredCertifications = item.PreferredCertifications;
            }
            else
            {
                ErrorMessage = "Không thể tải dữ liệu.";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Common/Login");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri("https://localhost:7004/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonSerializer.Serialize(Input), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync("api/jobcriterion", content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Employer/ListJobCriteria");
            }
            ErrorMessage = "Cập nhật thất bại.";
            return Page();
        }
    }
} 