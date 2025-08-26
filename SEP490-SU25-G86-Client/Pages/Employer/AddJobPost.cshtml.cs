using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO.OptionComboboxJobPostDTO;

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
        public List<CvTemplateDTO> CvTemplates { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Check quyền trước
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER")
            {
                return RedirectToPage("/NotFound");
            }

            // Set token nếu có
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            // Chỉ load dữ liệu nếu có quyền
            await LoadComboboxDataAsync();
            return Page();
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
            // NEW: Giới hạn độ dài để né người dùng dán quá nhiều HTML
            if (!string.IsNullOrEmpty(JobPost.Description) && JobPost.Description.Length > 20000)
            {
                ModelState.AddModelError("JobPost.Description", "Nội dung quá dài (tối đa 20.000 ký tự).");
                await LoadComboboxDataAsync();
                return Page();
            }

            // NEW: Sanitize HTML trước khi gửi API
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("ul");
            sanitizer.AllowedTags.Add("ol");
            sanitizer.AllowedTags.Add("li");
            sanitizer.AllowedTags.Add("strong");
            sanitizer.AllowedTags.Add("em");
            sanitizer.AllowedTags.Add("u");
            sanitizer.AllowedTags.Add("a");
            sanitizer.AllowedAttributes.Add("href");
            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");

            JobPost.Description = sanitizer.Sanitize(JobPost.Description ?? "");
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

            // Lấy danh sách CV template của employer
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
        public async Task<IActionResult> OnGetGetPositionsByIndustryAsync(int industryId)
        {
            try
            {
                var positions = await _httpClient.GetFromJsonAsync<List<JobPositionDTO>>(
                    $"api/jobpositions/by-industry/{industryId}");

                return new JsonResult(positions);
            }
            catch
            {
                return new JsonResult(new List<JobPositionDTO>());
            }
        }

    }
}
