using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SynonymService;
using System.Linq;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories
{
    public class JobPostRepository : IJobPostRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        private readonly ISynonymService _synonymService;
        public JobPostRepository(SEP490_G86_CvMatchContext context, ISynonymService synonymService)
        {
            _context = context;
            _synonymService = synonymService;
        }

        public async Task<(IEnumerable<JobPost> Posts, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null)
        {
            var query = _context.JobPosts
        .Include(j => j.Employer)
        .Include(j => j.SalaryRange)
        .Include(j => j.Province)
        .OrderByDescending(j => j.CreatedDate)
        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(j =>
                    j.Province != null &&
                    j.Province.Region != null &&
                    EF.Functions.Like(j.Province.Region, $"%{region}%"));
            }

            var totalItems = await query.CountAsync();

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalItems);
        }

        public async Task<IEnumerable<JobPost>> GetAllAsync()
        {
            return await _context.JobPosts
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<JobPost>> GetByEmployerIdAsync(int employerId)
        {
            return await _context.JobPosts
                .Where(j => j.EmployerId == employerId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<(IEnumerable<JobPost> Posts, int TotalItems)> GetFilteredJobPostsAsync(
            int page, int pageSize,
            int? provinceId = null,
            int? industryId = null,
            List<int>? employmentTypeIds = null,
            List<int>? experienceLevelIds = null,
            int? jobLevelId = null,
            int? minSalary = null,
            int? maxSalary = null,
            List<int>? datePostedRanges = null,
            string? keyword = null)
        {
            var query = _context.JobPosts
                .Include(j => j.Employer)
                .Include(j => j.Province)
                .Include(j => j.EmploymentType)
                .Include(j => j.ExperienceLevel)
                .Include(j => j.Industry)
                .Include(j => j.JobLevel)
                .Include(j => j.SalaryRange)
                .OrderByDescending(j => j.CreatedDate)
                .AsQueryable();

            // Apply filters
            if (provinceId.HasValue)
                query = query.Where(j => j.ProvinceId == provinceId.Value);
            if (industryId.HasValue)
                query = query.Where(j => j.IndustryId == industryId.Value);
            if (employmentTypeIds?.Any() == true)
                query = query.Where(j => employmentTypeIds.Contains((int)j.EmploymentTypeId));
            if (experienceLevelIds?.Any() == true)
                query = query.Where(j => experienceLevelIds.Contains((int)j.ExperienceLevelId));
            if (jobLevelId.HasValue)
                query = query.Where(j => j.JobLevelId == jobLevelId.Value);
            if (minSalary.HasValue)
                query = query.Where(j => j.SalaryRange!.MinSalary >= minSalary.Value);
            if (maxSalary.HasValue)
                query = query.Where(j => j.SalaryRange!.MaxSalary <= maxSalary.Value);
            if (datePostedRanges?.Any() == true)
            {
                var now = DateTime.UtcNow;
                var maxDays = datePostedRanges.Max();
                var filterDate = maxDays == 0 ? now.Date : now.AddDays(-maxDays);
                query = query.Where(j => j.CreatedDate >= filterDate);
            }

            // Nếu có keyword thì lọc bằng synonym và search
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var terms = _synonymService.ExpandKeywords(keyword);
                var allPosts = await query.ToListAsync();

                var filteredResult = allPosts.Where(j =>
                    terms.Any(k =>
                        j.Title != null &&
                        j.Title.Contains(k, StringComparison.OrdinalIgnoreCase)
                    )
                ).ToList();

                var totalItems = filteredResult.Count;

                var paged = filteredResult
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return (paged, totalItems);
            }

            // Nếu không có keyword thì trả như cũ
            var total = await query.CountAsync();

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, total);
        }
    }
}
