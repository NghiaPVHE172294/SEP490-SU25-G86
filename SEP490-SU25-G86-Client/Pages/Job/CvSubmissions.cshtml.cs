using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using System.Net.Http.Headers;
using System.Text.Json;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;

namespace SEP490_SU25_G86_Client.Pages.Job
{
    public class CvSubmissionsModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int JobPostId { get; set; }
        public List<CvSubmissionForJobPostDTO> CvSubmissions { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public string? JobPostTitle { get; set; }
        public string? CompanyName { get; set; }

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
                // Fetch job post details
                var jobPostResponse = await client.GetAsync($"https://localhost:7004/api/JobPosts/{JobPostId}");
                if (jobPostResponse.IsSuccessStatusCode)
                {
                    var jobPostJson = await jobPostResponse.Content.ReadAsStringAsync();
                    var jobPostDetail = JsonSerializer.Deserialize<ViewDetailJobPostDTO>(jobPostJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    JobPostTitle = jobPostDetail?.Title;
                    CompanyName = jobPostDetail?.CompanyName;
                }
                // Fetch CV submissions
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

        public async Task<IActionResult> OnPostApproveAsync(int id)
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent("\"APPROVED\"", System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7004/api/cvsubmissions/{id}/status", content);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Không thể duyệt CV.";
            }
            return RedirectToPage(new { JobPostId });
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent("\"REJECTED\"", System.Text.Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"https://localhost:7004/api/cvsubmissions/{id}/status", content);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Không thể từ chối CV.";
            }
            return RedirectToPage(new { JobPostId });
        }
    }
} 