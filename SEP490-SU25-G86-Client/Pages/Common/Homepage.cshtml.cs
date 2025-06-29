using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace SEP490_SU25_G86_Client.Pages
{
    public class HomepageModel : PageModel
    {
        public List<JobPostViewModel> JobPosts { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public string? Region { get; set; }

        public async Task OnGetAsync([FromQuery] int page = 1, string? region = null)
        {
            int pageSize = 9;
            CurrentPage = page < 1 ? 1 : page;
            Region = region;
            try
            {
                using var client = new HttpClient();
                // Lấy token từ session nếu có
                var token = HttpContext.Session.GetString("jwt_token");
                if (!string.IsNullOrEmpty(token))
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                // Gắn thêm region nếu có vào URL gọi API
                var url = $"https://localhost:7004/api/jobposts/homepage?page={CurrentPage}&pageSize={pageSize}";
                if (!string.IsNullOrEmpty(region))
                {
                    url += $"&region={region}";
                }

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<JobPostApiResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Nếu kết quả null hoặc không có job => lỗi dữ liệu
                    if (apiResponse == null || apiResponse.Jobs == null)
                    {
                        JobPosts = new List<JobPostViewModel>();
                        TotalPages = 1;
                        TotalItems = 0;
                        ModelState.AddModelError(string.Empty, "Failed to load job posts.");
                        return;
                    }

                    // Thành công: gán dữ liệu và tính phân trang
                    JobPosts = apiResponse.Jobs;
                    TotalItems = apiResponse.TotalItems;
                    TotalPages = (int)Math.Ceiling((double)apiResponse.TotalItems / pageSize);
                }
                else
                {
                    // Trường hợp API trả lỗi HTTP: 404, 500, 403, v.v.
                    JobPosts = new List<JobPostViewModel>();
                    TotalPages = 1;
                    TotalItems = 0;
                    ModelState.AddModelError(string.Empty, $"Failed to load job posts. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                // Trường hợp lỗi ngoại lệ: timeout, không kết nối được, lỗi JSON,...
                JobPosts = new List<JobPostViewModel>();
                TotalPages = 1;
                TotalItems = 0;
                ModelState.AddModelError(string.Empty, "An error occurred while loading job posts.");
            }
        }

        public class JobPostViewModel
        {
            public int JobPostId { get; set; }
            public string Title { get; set; }
            public string CompanyName { get; set; }
            public string Location { get; set; }
            public string Salary { get; set; }
        }

        private class JobPostApiResponse
        {
            public int TotalItems { get; set; }
            public List<JobPostViewModel> Jobs { get; set; }
        }
    }
}
