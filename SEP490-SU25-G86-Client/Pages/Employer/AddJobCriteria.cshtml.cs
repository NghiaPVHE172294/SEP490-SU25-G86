using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;

namespace SEP490_SU25_G86_Client.Pages.Employer
{
    public class AddJobCriteriaModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AddJobCriteriaModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int JobPostId { get; set; }
        [BindProperty]
        public AddJobCriterionDTO Input { get; set; } = new();
        public string Message { get; set; }
        public string? JobPostTitle { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/NotFound");
            Input.JobPostId = JobPostId;
            // Lấy title job post
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri("https://localhost:7004/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"api/jobposts/{JobPostId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var jobPost = JsonSerializer.Deserialize<SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO.JobPostListDTO>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                JobPostTitle = jobPost?.Title;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var role = HttpContext.Session.GetString("user_role");
            if (role != "EMPLOYER")
                return RedirectToPage("/NotFound");
            var token = HttpContext.Session.GetString("jwt_token");
            if (string.IsNullOrEmpty(token))
                return RedirectToPage("/Common/Login");
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new System.Uri("https://localhost:7004/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsJsonAsync("api/jobcriteria", Input);
            if (response.IsSuccessStatusCode)
            {
                Message = "Thêm tiêu chí thành công!";
                return Page();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return RedirectToPage("/Common/Login");
            }
            Message = "Có lỗi xảy ra khi thêm tiêu chí.";
            return Page();
        }
    }
} 