using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class CompanyDetailModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public CompanyDetailModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        public CompanyDto? Company { get; set; }
        public List<JobPostListDTO> JobPosts { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Lấy thông tin công ty
            var companyResponse = await _httpClient.GetAsync($"https://localhost:7004/api/Company/{id}");
            if (!companyResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Company = await companyResponse.Content.ReadFromJsonAsync<CompanyDto>();

            // Lấy danh sách jobposts của công ty
            var jobPostsResponse = await _httpClient.GetAsync($"https://localhost:7004/api/JobPosts/{id}/jobposts");
            if (jobPostsResponse.IsSuccessStatusCode)
            {
                JobPosts = await jobPostsResponse.Content.ReadFromJsonAsync<List<JobPostListDTO>>() ?? new List<JobPostListDTO>();
            }

            return Page();
        }
    }

    // DTOs
    public class CompanyDto
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Website { get; set; }
        public string CompanySize { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string IndustryName { get; set; } = null!;
        public int FollowersCount { get; set; }
    }

    public class JobPostListDTO
    {
        public int JobPostId { get; set; }
        public string Title { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? Salary { get; set; }
        public string? Location { get; set; }
        public string? EmploymentType { get; set; }
        public string? JobLevel { get; set; }
        public string? ExperienceLevel { get; set; }
        public string? Industry { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
