using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Job
{
    public class CvSubmissionsModel : PageModel
    {
        public class CvSubmissionForJobPostDTO
        {
            public int SubmissionId { get; set; }
            public DateTime? SubmissionDate { get; set; }
            public string CandidateName { get; set; } = null!;
            public string CvFileUrl { get; set; } = null!;
            public bool? IsShortlisted { get; set; }
            public string? RecruiterNote { get; set; }
        }

        [BindProperty(SupportsGet = true)]
        public int JobPostId { get; set; }
        public List<CvSubmissionForJobPostDTO> CvSubmissions { get; set; } = new();
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            if (JobPostId <= 0)
            {
                ErrorMessage = "Thiếu JobPostId.";
                return;
            }
            try
            {
                var client = new HttpClient();
                var token = HttpContext.Session.GetString("jwt_token");
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var apiUrl = $"https://localhost:7004/api/cvsubmissions/jobpost/{JobPostId}";
                var response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    CvSubmissions = JsonSerializer.Deserialize<List<CvSubmissionForJobPostDTO>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
                else
                {
                    ErrorMessage = $"Không thể tải danh sách CV: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Lỗi: {ex.Message}";
            }
        }
    }
} 