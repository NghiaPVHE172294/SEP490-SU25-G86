using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class ListJobsModel : PageModel
    {
        private readonly HttpClient _httpClient;
        public List<JobDto> Jobs { get; set; } = new();
        public List<Province> Provinces { get; set; } = new();
        public int? ProvinceId { get; set; }
        public List<Industry> Industries { get; set; } = new();
        public int? IndustryId { get; set; }
        public List<EmploymentType> EmploymentTypes { get; set; } = new();
        public List<int> SelectedEmploymentTypeIds { get; set; } = new();
        public List<int> SelectedExperienceLevelIds { get; set; } = new();
        public List<ExperienceLevel> ExperienceLevels { get; set; } = new();
        public List<int> SelectedDateRanges { get; set; } = new();
        public int? MinSalaryInput { get; set; }
        public int? MaxSalaryInput { get; set; }
        [BindProperty(SupportsGet = true)]
        public string? Keyword { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public List<CvDTO> MyCvs { get; set; } = new List<CvDTO>();

        public ListJobsModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7004/");
        }

        public async Task OnGetAsync(
            [FromQuery] int page = 1,
            [FromQuery] int? provinceId = null,
            [FromQuery] int? industryId = null,
            [FromQuery] List<int>? employmentTypeIds = null,
            [FromQuery] List<int>? experienceLevelIds = null,
            [FromQuery] List<int>? datePostedRanges = null,
            [FromQuery] List<string>? salary_range = null,
            [FromQuery] string? keyword = null)
        {
            Keyword = keyword;
            ProvinceId = provinceId;
            IndustryId = industryId;
            SelectedEmploymentTypeIds = employmentTypeIds ?? new();
            SelectedExperienceLevelIds = experienceLevelIds ?? new();
            SelectedDateRanges = datePostedRanges ?? new();
            int? minSalary = null;
            int? maxSalary = null;

            if (salary_range != null && salary_range.Any(r => !string.IsNullOrWhiteSpace(r)))
            {
                var mins = new List<int>();
                var maxs = new List<int>();

                foreach (var r in salary_range.Where(r => !string.IsNullOrWhiteSpace(r)))
                {

                    var parts = r.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[0], out var min) && int.TryParse(parts[1], out var max))
                    {
                        mins.Add(min);
                        maxs.Add(max);
                    }
                }

                if (mins.Any()) minSalary = mins.Min();
                if (maxs.Any()) maxSalary = maxs.Max();
            }

            int pageSize = 10;
            CurrentPage = page < 1 ? 1 : page;

            using var client = new HttpClient();
            var token = HttpContext.Session.GetString("jwt_token");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var url = $"https://localhost:7004/api/jobposts/viewall?page={page}&pageSize={pageSize}";
            //Add thêm filter nếu có
            if (!string.IsNullOrWhiteSpace(Keyword))
                url += $"&keyword={Uri.EscapeDataString(Keyword)}";
            if (provinceId.HasValue)
                url += $"&provinceId={provinceId.Value}";
            if (industryId.HasValue)
                url += $"&industryId={industryId.Value}";
            if (SelectedEmploymentTypeIds.Any())
            {
                foreach (var id in SelectedEmploymentTypeIds)
                {
                    url += $"&employmentTypeIds={id}";
                }
            }
            if (SelectedExperienceLevelIds.Any())
            {
                foreach (var id in SelectedExperienceLevelIds)
                    url += $"&experienceLevelIds={id}";
            }
            if (SelectedDateRanges.Any())
            {
                foreach (var d in SelectedDateRanges)
                    url += $"&datePostedRanges={d}";
            }
            if (minSalary.HasValue)
                url += $"&minSalary={minSalary.Value}";
            if (maxSalary.HasValue)
                url += $"&maxSalary={maxSalary.Value}";
            try
            {
                var response = await client.GetFromJsonAsync<JobPostApiResponse>(url, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (response != null && response.Posts != null)
                {
                    Jobs = response.Posts;
                    TotalItems = response.TotalItems;
                    TotalPages = (int)Math.Ceiling((double)TotalItems / pageSize);
                }

                await LoadProvincesAsync();
                await LoadIndustriesAsync();
                await LoadExperienceLevelsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching jobs: {ex.Message}");
            }
            // Lấy danh sách CV nếu đã đăng nhập
            var userIdStr = HttpContext.Session.GetString("userId");
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                var cvRes = await _httpClient.GetAsync("api/Cv/my");
                if (cvRes.IsSuccessStatusCode)
                {
                    var cvContent = await cvRes.Content.ReadAsStringAsync();
                    MyCvs = JsonSerializer.Deserialize<List<CvDTO>>(cvContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<CvDTO>();
                }
            }
        }
        private async Task LoadProvincesAsync()
        {
            using var client = new HttpClient();
            var provinces = await client.GetFromJsonAsync<List<Province>>("https://localhost:7004/api/provinces");
            Provinces = provinces ?? new();
        }
        private async Task LoadIndustriesAsync()
        {
            using var client = new HttpClient();
            var industries = await client.GetFromJsonAsync<List<Industry>>("https://localhost:7004/api/industries");
            Industries = industries ?? new();
        }

        private async Task LoadExperienceLevelsAsync()
        {
            using var client = new HttpClient();
            var expLevels = await client.GetFromJsonAsync<List<ExperienceLevel>>("https://localhost:7004/api/experiencelevels");
            ExperienceLevels = expLevels ?? new();
        }
        public class JobDto
        {
            [JsonPropertyName("jobPostId")]
            public int JobPostId { get; set; }
            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("companyName")]
            public string CompanyName { get; set; }

            [JsonPropertyName("industry")]
            public string Category { get; set; }

            [JsonPropertyName("employmentType")]
            public string JobType { get; set; }

            [JsonPropertyName("salary")]
            public string SalaryRange { get; set; }

            [JsonPropertyName("location")]
            public string Location { get; set; }

            [JsonPropertyName("createdDate")]
            public string TimePosted { get; set; }

            [JsonPropertyName("experienceLevel")]
            public string Experience { get; set; }
            [JsonPropertyName("isApplied")]
            public bool IsApplied { get; set; }
            [JsonPropertyName("companyLogoUrl")]
            public string CompanyLogoUrl { get; set; }
            //public string FormattedSalary
            //{
            //    get
            //    {
            //        var parts = SalaryRange.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            //        if (parts.Length >= 4)
            //        {
            //            return $"{parts[0]} {parts[1]} {parts[2]} Triệu {parts[3]}";
            //        }
            //        return SalaryRange; 
            //    }
            //}
            [JsonPropertyName("daysSincePostedOrUpdated")]
            public string DaysSincePostedOrUpdated { get; set; }
            [JsonPropertyName("companyId")]
            public int? CompanyId { get; set; }
        }
        public class JobPostApiResponse
        {
            [JsonPropertyName("posts")]
            public List<JobDto> Posts { get; set; }

            [JsonPropertyName("totalItems")]
            public int TotalItems { get; set; }
        }
        public class Province
        {
            public int ProvinceId { get; set; }
            public string ProvinceName { get; set; }
            public string? Region { get; set; }
        }
        public class Industry
        {
            public int IndustryId { get; set; }
            public string IndustryName { get; set; }
        }
        public class EmploymentType
        {
            public int EmploymentTypeId { get; set; }
            public string EmploymentTypeName { get; set; } = string.Empty;
        }
        public class ExperienceLevel
        {
            public int ExperienceLevelId { get; set; }
            public string ExperienceLevelName { get; set; } = string.Empty;
            public int? MinYears { get; set; }

        }
    }
}
