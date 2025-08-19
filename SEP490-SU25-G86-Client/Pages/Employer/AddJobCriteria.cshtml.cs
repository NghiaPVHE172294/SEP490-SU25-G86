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

        public async Task<IActionResult> OnGetAsync()
        {
            // 1) Token
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Common/Login");

            // 2) Kiểm tra JobPost thuộc quyền & lấy Title
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7004/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await client.GetAsync($"api/jobposts/{JobPostId}");
            if (!res.IsSuccessStatusCode)
            {
                if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                    ErrorMessage = "Bài đăng công việc không tồn tại.";
                else if (res.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    ErrorMessage = "Bạn không sở hữu bài đăng này.";
                else
                    ErrorMessage = "Không thể tải thông tin bài đăng.";
                return Page();
            }

            var json = await res.Content.ReadAsStringAsync();
            var jobPost = JsonSerializer.Deserialize<JobPostListDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            JobPostTitle = jobPost?.Title;

            // 3) Gán JobPostId vào Input để hiển thị/giữ state form
            Input.JobPostId = JobPostId;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // 1) Gán lại để chắc chắn
            if (JobPostId > 0) Input.JobPostId = JobPostId;
            if (!ModelState.IsValid) return Page();

            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token)) return RedirectToPage("/Common/Login");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7004/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // (Tuỳ chọn) Kiểm tra lại quyền trước khi POST – trải nghiệm tốt hơn
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

            // 2) GỌI ĐÚNG ROUTE SỐ ÍT
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
            ErrorMessage = string.IsNullOrWhiteSpace(err)
                ? "Có lỗi xảy ra khi thêm tiêu chí."
                : $"Có lỗi xảy ra khi thêm tiêu chí: {err}";
            return Page();
        }
    }
}
