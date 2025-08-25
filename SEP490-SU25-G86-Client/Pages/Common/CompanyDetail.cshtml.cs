using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Security.Claims;
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
        public bool IsFollowing { get; set; }
        public bool IsBlocked { get; set; }
        public int Id { get; set; }
        public string? AccountId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }

        public async Task<IActionResult> OnGetAsync([FromQuery] int id, [FromQuery] int page = 1)
        {
            UserId = HttpContext.Session.GetString("userId");
            Role = HttpContext.Session.GetString("user_role");
            if (!HttpContext.Request.Query.TryGetValue("id", out var idValue) || !int.TryParse(idValue, out var parsedId) || parsedId <= 0)
            {
                return BadRequest("Invalid company ID");
            }

            id = parsedId;
            Id = id;

            const int pageSize = 5;
            CurrentPage = page < 1 ? 1 : page;

            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                AccountId = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (jwtToken.ValidTo < DateTime.UtcNow)
                {
                    AccountId = null;
                }
            }

            // Lấy thông tin công ty
            var companyResponse = await _httpClient.GetAsync($"https://localhost:7004/api/Company/{id}");
            if (!companyResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }
            Company = await companyResponse.Content.ReadFromJsonAsync<CompanyDto>();
            if (Company == null)
            {
                return NotFound();
            }

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

            // Kiểm tra trạng thái theo dõi công ty
            if (!string.IsNullOrEmpty(AccountId))
            {
                var checkUrl = $"https://localhost:7004/api/User/{id}/check-follow-block?accountId={AccountId}";
                var checkResponse = await _httpClient.GetAsync(checkUrl);
                if (checkResponse.IsSuccessStatusCode)
                {
                    var json = await checkResponse.Content.ReadAsStringAsync();
                    var checkResult = JsonSerializer.Deserialize<FollowBlockStatusDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (checkResult != null)
                    {
                        IsFollowing = checkResult.IsFollowing;
                        IsBlocked = checkResult.IsBlocked;
                    }
                }
            }
            else
            {
                IsFollowing = false;
                IsBlocked = false;
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
    public class FollowBlockStatusDto
    {
        [JsonPropertyName("isFollowing")]
        public bool IsFollowing { get; set; }

        [JsonPropertyName("isBlocked")]
        public bool IsBlocked { get; set; }
    }
}
