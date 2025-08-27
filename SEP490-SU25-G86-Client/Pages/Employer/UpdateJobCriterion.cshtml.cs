using Ganss.Xss;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
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
                Input.Address = item.Address;
                Input.Summary = item.Summary;
                Input.WorkHistory = item.WorkHistory;
                Input.Projects = item.Projects;
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
            // 1) Chuẩn hoá text ngắn
            Input.RequiredSkills = StripHtml(Input.RequiredSkills);
            Input.EducationLevel = StripHtml(Input.EducationLevel);
            Input.RequiredJobTitles = StripHtml(Input.RequiredJobTitles);
            Input.PreferredLanguages = StripHtml(Input.PreferredLanguages);
            Input.Address = StripHtml(Input.Address);

            // số -> chỉ validate, không strip
            if (Input.RequiredExperience is < 0 or > 50)
                ModelState.AddModelError("Input.RequiredExperience", "Kinh nghiệm phải từ 0–50 năm.");

            // 2) Rich text: sanitize
            var rich = new HtmlSanitizer();
            rich.AllowedTags.UnionWith(new[] { "p", "ul", "ol", "li", "strong", "em", "u", "br", "a" });
            rich.AllowedAttributes.Add("href");
            rich.AllowedSchemes.UnionWith(new[] { "http", "https" });

            Input.Summary = rich.Sanitize(Input.Summary ?? "");
            Input.WorkHistory = rich.Sanitize(Input.WorkHistory ?? "");
            Input.PreferredCertifications = rich.Sanitize(Input.PreferredCertifications ?? "");
            Input.Projects = rich.Sanitize(Input.Projects ?? "");

            // 3) Validate độ dài sau sanitize
            if (Input.Summary?.Length > 5000)
                ModelState.AddModelError("Input.Summary", "Tối đa 5.000 ký tự.");
            if (Input.WorkHistory?.Length > 10000)
                ModelState.AddModelError("Input.WorkHistory", "Tối đa 10.000 ký tự.");
            if (Input.PreferredCertifications?.Length > 5000)
                ModelState.AddModelError("Input.PreferredCertifications", "Tối đa 5.000 ký tự.");
            if (Input.Projects?.Length > 5000)
                ModelState.AddModelError("Input.Projects", "Tối đa 5.000 ký tự.");

            if (!ModelState.IsValid) return Page();
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
        private static string StripHtml(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            var text = Regex.Replace(html, "<.*?>", string.Empty);
            text = System.Net.WebUtility.HtmlDecode(text);
            return text.Trim();
        }
    }
} 