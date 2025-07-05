using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.Json.Serialization;
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
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }

        public async Task<IActionResult> OnGetAsync(int id, [FromQuery] int page = 1)
        {
            const int pageSize = 5;
            CurrentPage = page < 1 ? 1 : page;

            // Lấy thông tin công ty
            var companyResponse = await _httpClient.GetAsync($"https://localhost:7004/api/Company/{id}");
            if (!companyResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Company = await companyResponse.Content.ReadFromJsonAsync<CompanyDto>();

            // Lấy danh sách jobposts có phân trang
            var jobPostsResponse = await _httpClient.GetAsync(
                $"https://localhost:7004/api/JobPosts/{id}/jobposts?page={CurrentPage}&pageSize={pageSize}");

            if (jobPostsResponse.IsSuccessStatusCode)
            {
                var json = await jobPostsResponse.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JobPostApiResponse>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result != null)
                {
                    JobPosts = result.Items ?? new List<JobPostListDTO>();
                    TotalItems = result.TotalItems;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / pageSize);
                }
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
    public class JobPostApiResponse
    {
        [JsonPropertyName("posts")]
        public List<JobPostListDTO>? Items { get; set; }
        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }
    }
}
