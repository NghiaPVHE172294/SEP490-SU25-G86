using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SEP490_SU25_G86_API.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SEP490_SU25_G86_Client.Pages.Common
{
    public class ListJobsModel : PageModel
    {
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
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public async Task OnGetAsync(
            [FromQuery] int page = 1,
            [FromQuery] int? provinceId = null,
            [FromQuery] int? industryId = null,
            [FromQuery] List<int>? employmentTypeIds = null,
            [FromQuery] List<int>? experienceLevelIds = null,
            [FromQuery] List<int>? datePostedRanges = null,
            [FromQuery] List<int>? salary_min = null,
            [FromQuery] List<int>? salary_max = null)
        {
            ProvinceId = provinceId;
            IndustryId = industryId;
            SelectedEmploymentTypeIds = employmentTypeIds ?? new();
            SelectedExperienceLevelIds = experienceLevelIds ?? new();
            SelectedDateRanges = datePostedRanges ?? new();
            int? minSalary = null;
            int? maxSalary = null;

            if (salary_min != null && salary_min.Any())
                minSalary = salary_min.Min();
            if (salary_max != null && salary_max.Any())
                maxSalary = salary_max.Max();

            int pageSize = 5;
            CurrentPage = page < 1 ? 1 : page;

            using var client = new HttpClient();
            var url = $"https://localhost:7004/api/jobposts/viewall?page={page}&pageSize={pageSize}";
            //Add thêm filter nếu có
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
            MinSalaryInput = salary_min?.Min();
            MaxSalaryInput = salary_max?.Max();
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching jobs: {ex.Message}");
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
        public class JobDto
        {
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
