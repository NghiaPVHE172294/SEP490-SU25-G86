using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserService;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class ListJobByEmployerModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<JobPostListDTO> Jobs { get; set; } = new();
        public string? CompanyName { get; set; }
        public string? PhoneNumber { get; set; }
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
                return RedirectToPage("/NotFound");

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            UserProfileDTO? profile = null;
            var profileRes = await _httpClient.GetAsync("api/User/profile"); // lưu ý: chữ U viết hoa
            if (profileRes.IsSuccessStatusCode)
            {
                profile = JsonSerializer.Deserialize<UserProfileDTO>(
                    await profileRes.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                PhoneNumber = profile?.Phone; // <- phone của user
            }

            // 2) Lấy thông tin công ty như trang kia của bạn
            CompanyDetailDTO? company = null; // hoặc CompanyDTO nếu bạn muốn thống nhất
            var companyRes = await _httpClient.GetAsync("api/Companies/me");
            if (companyRes.IsSuccessStatusCode)
            {
                company = JsonSerializer.Deserialize<CompanyDetailDTO>(
                    await companyRes.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                company = null; // giữ nguyên cách bạn đang làm
            }

            // 3) Gán sang biến ViewModel đang dùng ở View
            CompanyName = company?.CompanyName;
            // Gọi API lấy danh sách JobPosts
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
                JobPostIdsWithCriteria = jobCriteria?.Select(c => c.JobPostId).ToHashSet() ?? new();
            }

            // Phân trang
            TotalRecords = Jobs.Count;
            Jobs = Jobs
                .OrderByDescending(j => j.CreatedDate)
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
                TempData["ToastError"] = string.IsNullOrWhiteSpace(msg)
                    ? $"Xóa thất bại ({(int)apiResponse.StatusCode})"
                    : msg;
            }

            return RedirectToPage(new { StatusFilter });
        }
    }
}
