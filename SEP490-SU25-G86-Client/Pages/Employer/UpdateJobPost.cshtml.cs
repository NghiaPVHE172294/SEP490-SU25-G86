using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO.OptionComboboxJobPostDTO;

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
            // ===== 0) Chuẩn hoá input =====
            JobPost.Title = JobPost.Title?.Trim();
            JobPost.WorkLocation = JobPost.WorkLocation?.Trim();
            JobPost.Description = JobPost.Description ?? "";
            JobPost.CandidaterRequirements = JobPost.CandidaterRequirements ?? "";
            JobPost.Interest = JobPost.Interest ?? "";

            // ===== 1) Tạo sanitizer =====
            var richSanitizer = new HtmlSanitizer();
            richSanitizer.AllowedTags.UnionWith(new[] { "p", "ul", "ol", "li", "strong", "em", "u", "br", "a" });
            richSanitizer.AllowedAttributes.Add("href");
            richSanitizer.AllowedSchemes.UnionWith(new[] { "http", "https" });

            // Text-only: strip hết tag (nếu ai đó dán HTML vào tiêu đề/địa điểm)
            JobPost.Title = StripHtml(JobPost.Title);
            JobPost.WorkLocation = StripHtml(JobPost.WorkLocation);

            // Rich-text: giữ format nhưng an toàn
            JobPost.Description = richSanitizer.Sanitize(JobPost.Description);
            JobPost.CandidaterRequirements = richSanitizer.Sanitize(JobPost.CandidaterRequirements);
            JobPost.Interest = richSanitizer.Sanitize(JobPost.Interest);

            // ===== 2) Validate sau khi sanitize/strip =====
            if (JobPost.Title.Length is < 2 or > 255)
                ModelState.AddModelError("JobPost.Title", "Tiêu đề phải 2–255 ký tự.");
            if (JobPost.WorkLocation.Length is < 2 or > 255)
                ModelState.AddModelError("JobPost.WorkLocation", "Địa điểm phải 2–255 ký tự.");

            const int MaxHtmlLen = 20000;
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
        private static string StripHtml(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            var text = Regex.Replace(html, "<.*?>", string.Empty);
            text = System.Net.WebUtility.HtmlDecode(text);
            return text.Trim();
        }
    }
}