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
            // ===== 0) Chuẩn hoá input trước =====
            JobPost.Title = JobPost.Title?.Trim();
            JobPost.WorkLocation = JobPost.WorkLocation?.Trim();
            JobPost.Description = JobPost.Description ?? "";
            JobPost.CandidaterRequirements = JobPost.CandidaterRequirements ?? "";
            JobPost.Interest = JobPost.Interest ?? "";

            // ===== 1) Tạo sanitizer =====
            var richSanitizer = new HtmlSanitizer();
            // whitelist cho nội dung dài (TinyMCE)
            richSanitizer.AllowedTags.UnionWith(new[] { "p", "ul", "ol", "li", "strong", "em", "u", "br", "a" });
            richSanitizer.AllowedAttributes.Add("href");
            richSanitizer.AllowedSchemes.UnionWith(new[] { "http", "https" });

            // plain = xoá hết tag (chỉ giữ text)
            var plainSanitizer = new HtmlSanitizer();
            plainSanitizer.AllowedTags.Clear();
            plainSanitizer.AllowedAttributes.Clear();
            plainSanitizer.AllowedSchemes.Clear();

            // ===== 2) Sanitize theo loại field =====
            // Text-only (Title/WorkLocation): giữ chữ, loại bỏ HTML
            JobPost.Title = plainSanitizer.Sanitize(JobPost.Title ?? "");
            JobPost.WorkLocation = plainSanitizer.Sanitize(JobPost.WorkLocation ?? "");

            // Rich-text (các textarea có TinyMCE)
            JobPost.Description = richSanitizer.Sanitize(JobPost.Description);
            JobPost.CandidaterRequirements = richSanitizer.Sanitize(JobPost.CandidaterRequirements);
            JobPost.Interest = richSanitizer.Sanitize(JobPost.Interest);

            // ===== 3) Validate độ dài sau khi sanitize =====
            int MaxHtmlLen = 20000;
            if (JobPost.Description.Length > MaxHtmlLen)
                ModelState.AddModelError("JobPost.Description", $"Nội dung quá dài (tối đa {MaxHtmlLen:N0} ký tự).");
            if (JobPost.CandidaterRequirements.Length > MaxHtmlLen)
                ModelState.AddModelError("JobPost.CandidaterRequirements", $"Nội dung quá dài (tối đa {MaxHtmlLen:N0} ký tự).");
            if (JobPost.Interest.Length > MaxHtmlLen)
                ModelState.AddModelError("JobPost.Interest", $"Nội dung quá dài (tối đa {MaxHtmlLen:N0} ký tự).");

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Vui lòng nhập đầy đủ thông tin bắt buộc.";
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
