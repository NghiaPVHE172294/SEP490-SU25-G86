using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using SEP490_SU25_G86_Client.Pages.Common;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class AddJobCriteriaModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AddJobCriteriaModel(IHttpClientFactory httpClientFactory) => _httpClientFactory = httpClientFactory;

        [BindProperty(SupportsGet = true)] public int JobPostId { get; set; }
        [BindProperty] public AddJobCriterionDTO Input { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? JobPostTitle { get; set; }

        private async Task<bool> LoadJobPostTitleAsync(string token)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7004/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync($"api/jobposts/{JobPostId}");
            if (!res.IsSuccessStatusCode)
            {
                ErrorMessage = res.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => "Bài đăng công việc không tồn tại.",
                    System.Net.HttpStatusCode.Forbidden => "Bạn không sở hữu bài đăng này.",
                    _ => "Không thể tải thông tin bài đăng."
                };
                return false;
            }

            var json = await res.Content.ReadAsStringAsync();
            var jobPost = JsonSerializer.Deserialize<JobPostListDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            JobPostTitle = jobPost?.Title;
            return true;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Common/Login");

            var ok = await LoadJobPostTitleAsync(token);
            if (!ok) return Page();

            Input.JobPostId = JobPostId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Common/Login");

            // load lại JobPostTitle để luôn hiển thị trong view
            var ok = await LoadJobPostTitleAsync(token);
            if (!ok) return Page();

            if (JobPostId > 0) Input.JobPostId = JobPostId;
            if (!ModelState.IsValid) return Page();

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7004/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // check quyền
            var check = await client.GetAsync($"api/jobposts/{Input.JobPostId}");
            if (!check.IsSuccessStatusCode)
            {
                ErrorMessage = check.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => "Bài đăng công việc không tồn tại.",
                    System.Net.HttpStatusCode.Forbidden => "Bạn không sở hữu bài đăng này.",
                    _ => "Không thể xác thực quyền trên bài đăng."
                };
                return Page();
            }

            // gọi API thêm
            var response = await client.PostAsJsonAsync("api/jobcriterion", Input);

            if (response.IsSuccessStatusCode)
                return RedirectToPage("/Employer/ListJobCriteria");

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToPage("/Common/Login");

            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                ErrorMessage = "Bạn không có quyền thêm tiêu chí cho bài đăng này.";
                return Page();
            }

            var err = await response.Content.ReadAsStringAsync();
            try
            {
                var problem = JsonSerializer.Deserialize<ValidationProblemDetails>(err,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (problem?.Errors != null && problem.Errors.Any())
                {
                    ErrorMessage = string.Join("<br/>",
                        problem.Errors.SelectMany(e => e.Value));
                }
                else
                {
                    ErrorMessage = problem?.Title ?? "Có lỗi xảy ra khi thêm tiêu chí.";
                }
            }
            catch
            {
                ErrorMessage = string.IsNullOrWhiteSpace(err)
                    ? "Có lỗi xảy ra khi thêm tiêu chí."
                    : $"Có lỗi xảy ra khi thêm tiêu chí: {err}";
            }

            return Page();
        }
    }
}
