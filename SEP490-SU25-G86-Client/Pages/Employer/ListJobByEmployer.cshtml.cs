using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using System.Text.Json;
using System.Net.Http.Headers;
namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class ListJobByEmployerModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<JobPostListDTO> Jobs { get; set; } = new();
        public HashSet<int> JobPostIdsWithCriteria { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public ListJobByEmployerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task<IActionResult> OnGetAsync()
        {

            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER")
            {
                return RedirectToPage("/NotFound");
            }

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Lấy toàn bộ JobPost
            var response = await _httpClient.GetAsync("api/JobPosts/employer");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Jobs = JsonSerializer.Deserialize<List<JobPostListDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();

                // Lọc theo trạng thái nếu có
                if (!string.IsNullOrEmpty(StatusFilter))
                {
                    Jobs = Jobs
                        .Where(j => j.Status != null && j.Status.Equals(StatusFilter, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }
            }

            // Lấy toàn bộ JobCriteria
            var criteriaResponse = await _httpClient.GetAsync("api/jobcriterion/my");
            if (criteriaResponse.IsSuccessStatusCode)
            {
                var criteriaContent = await criteriaResponse.Content.ReadAsStringAsync();
                var jobCriteria = JsonSerializer.Deserialize<List<JobCriterionDTO>>(criteriaContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                JobPostIdsWithCriteria = jobCriteria != null ? jobCriteria.Select(c => c.JobPostId).ToHashSet() : new();
            }

            // Phân trang
            TotalRecords = Jobs.Count;
            Jobs = Jobs
                .OrderByDescending(j => j.CreatedDate) // hoặc .CreatedAt tùy DTO
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
            return Page();
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER")
                return RedirectToPage("/NotFound");

            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ToastError"] = "Bạn cần đăng nhập để xóa tin.";
                return RedirectToPage(new { StatusFilter });
            }

            // Gắn Bearer token rồi gọi đúng endpoint: https://localhost:7004/api/JobPosts/{id}
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var apiResponse = await _httpClient.DeleteAsync($"api/JobPosts/{id}");

            if (apiResponse.StatusCode == System.Net.HttpStatusCode.NoContent)
                TempData["ToastSuccess"] = "Đã xóa tin tuyển dụng.";
            else if (apiResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                TempData["ToastWarning"] = "Tin tuyển dụng không tồn tại hoặc đã bị xóa.";
            else if (apiResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                TempData["ToastError"] = "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.";
            else if (apiResponse.StatusCode == System.Net.HttpStatusCode.Forbidden)
                TempData["ToastError"] = "Bạn không có quyền xóa tin này.";
            else
            {
                var msg = await apiResponse.Content.ReadAsStringAsync();
                TempData["ToastError"] = string.IsNullOrWhiteSpace(msg) ? $"Xóa thất bại ({(int)apiResponse.StatusCode})" : msg;
            }

            return RedirectToPage(new { StatusFilter });
        }
    }
}
