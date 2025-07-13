using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using System.Text;
using System.Text.Json;
using static SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO.OptionComboboxJobPostDTO;
using System.Net.Http.Json;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class AddJobPostModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public AddJobPostModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        [BindProperty]
        public AddJobPostDTO JobPost { get; set; } = new();

        public string ErrorMessage { get; set; }

        public int JobPostId { get; set; }

        public List<EmploymentTypeDTO> EmploymentTypes { get; set; } = new();
        public List<JobPositionDTO> JobPositions { get; set; } = new();
        public List<ProvinceDTO> Provinces { get; set; } = new();
        public List<ExperienceLevelDTO> ExperienceLevels { get; set; } = new();
        public List<JobLevelDTO> JobLevels { get; set; } = new();
        public List<IndustryDTO> Industries { get; set; } = new();
        public List<SalaryRangeDTO> SalaryRanges { get; set; } = new();

        public async Task OnGetAsync()
        {
            await LoadComboboxDataAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        ErrorMessage += $" [{key}: {error.ErrorMessage}]";
                    }
                }

                await LoadComboboxDataAsync();
                return Page();
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var content = new StringContent(JsonSerializer.Serialize(JobPost), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("api/jobposts", content);
                if (response.IsSuccessStatusCode)
                {
                    var resp = await response.Content.ReadAsStringAsync();
                    var job = JsonDocument.Parse(resp).RootElement;
                    int jobPostId = job.GetProperty("jobPostId").GetInt32();
                    JobPostId = jobPostId;
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
        }
    }
}
