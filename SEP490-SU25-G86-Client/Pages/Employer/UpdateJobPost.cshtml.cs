using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using System.Text;
using System.Text.Json;
using static SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO.OptionComboboxJobPostDTO;
using System.Net.Http.Json;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class UpdateJobPostModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public UpdateJobPostModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        [BindProperty]
        public UpdateJobPostDTO JobPost { get; set; } = new();

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public List<EmploymentTypeDTO> EmploymentTypes { get; set; } = new();
        public List<JobPositionDTO> JobPositions { get; set; } = new();
        public List<ProvinceDTO> Provinces { get; set; } = new();
        public List<ExperienceLevelDTO> ExperienceLevels { get; set; } = new();
        public List<JobLevelDTO> JobLevels { get; set; } = new();
        public List<IndustryDTO> Industries { get; set; } = new();
        public List<SalaryRangeDTO> SalaryRanges { get; set; } = new();
        public List<CvTemplateDTO> CvTemplates { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int jobPostId)
        {
            await LoadComboboxDataAsync();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"api/JobPosts/{jobPostId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var detail = JsonSerializer.Deserialize<ViewDetailJobPostDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (detail != null)
                {
                    JobPost.JobPostId = detail.JobPostId;
                    JobPost.Title = detail.Title;
                    JobPost.WorkLocation = detail.WorkLocation;
                    JobPost.Status = detail.Status;
                    JobPost.EndDate = detail.EndDate;
                    JobPost.Description = detail.Description;
                    JobPost.CandidaterRequirements = detail.CandidaterRequirements;
                    JobPost.Interest = detail.Interest;
                    JobPost.IndustryId = detail.IndustryId;
                    JobPost.JobPositionId = detail.JobPositionId;
                    JobPost.SalaryRangeId = detail.SalaryRangeId;
                    JobPost.ProvinceId = detail.ProvinceId;
                    JobPost.ExperienceLevelId = detail.ExperienceLevelId;
                    JobPost.JobLevelId = detail.JobLevelId;
                    JobPost.EmploymentTypeId = detail.EmploymentTypeId;
                    JobPost.IsAienabled = detail.IsAienabled;
                    JobPost.CvtemplateOfEmployerId = detail.CvTemplateId;
                }
            }
            else
            {
                ErrorMessage = "Không tìm thấy bài đăng hoặc lỗi hệ thống.";
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                await LoadComboboxDataAsync();
                return Page();
            }
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(JobPost), Encoding.UTF8, "application/json");
            try
            {
                var response = await _httpClient.PutAsync("api/jobposts", content);
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Cập nhật thành công!";
                    await LoadComboboxDataAsync();
                    return Page();
                }
                else
                {
                    ErrorMessage = $"Lỗi: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi hệ thống: {ex.Message}";
            }
            await LoadComboboxDataAsync();
            return Page();
        }

        private async Task LoadComboboxDataAsync()
        {
            EmploymentTypes = await _httpClient.GetFromJsonAsync<List<EmploymentTypeDTO>>("api/employmenttypes");
            JobPositions = await _httpClient.GetFromJsonAsync<List<JobPositionDTO>>("api/jobpositions");
            Provinces = await _httpClient.GetFromJsonAsync<List<ProvinceDTO>>("api/provinces");
            ExperienceLevels = await _httpClient.GetFromJsonAsync<List<ExperienceLevelDTO>>("api/experiencelevels");
            JobLevels = await _httpClient.GetFromJsonAsync<List<JobLevelDTO>>("api/joblevels");
            Industries = await _httpClient.GetFromJsonAsync<List<IndustryDTO>>("api/industries");
            SalaryRanges = await _httpClient.GetFromJsonAsync<List<SalaryRangeDTO>>("api/salaryranges");
            CvTemplates = new List<CvTemplateDTO>();
            try
            {
                var token = HttpContext.Session.GetString("jwt_token");
                if (!string.IsNullOrEmpty(token))
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var templates = await _httpClient.GetFromJsonAsync<List<CvTemplateDTO>>("api/employer/cv-templates");
                if (templates != null)
                    CvTemplates = templates;
            }
            catch { }
        }
    }
}