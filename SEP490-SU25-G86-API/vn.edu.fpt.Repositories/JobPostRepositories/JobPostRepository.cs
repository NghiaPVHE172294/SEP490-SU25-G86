using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using System.Linq;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories
{
    public class JobPostRepository : IJobPostRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public JobPostRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
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
            List<int>? datePostedRanges = null)
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

            if (provinceId.HasValue)
                query = query.Where(j => j.ProvinceId == provinceId.Value);
            if (industryId.HasValue)
                query = query.Where(j => j.IndustryId == industryId.Value);

            if (employmentTypeIds != null && employmentTypeIds.Any())
                query = query.Where(j => employmentTypeIds.Contains((int)j.EmploymentTypeId));

            if (experienceLevelIds != null && experienceLevelIds.Any())
                query = query.Where(j => experienceLevelIds.Contains((int)j.ExperienceLevelId));

            if (jobLevelId.HasValue)
                query = query.Where(j => j.JobLevelId == jobLevelId.Value);
            if (minSalary.HasValue)
                query = query.Where(j => j.SalaryRange!.MinSalary >= minSalary.Value);
            if (maxSalary.HasValue)
                query = query.Where(j => j.SalaryRange!.MaxSalary <= maxSalary.Value);

            // Lọc theo ngày đăng
            if (datePostedRanges != null && datePostedRanges.Any())
            {
                var now = DateTime.UtcNow;

                var maxDays = datePostedRanges.Max();

                var filterDate = maxDays == 0 ? now.Date : now.AddDays(-maxDays);

                query = query.Where(j => j.CreatedDate >= filterDate);
            }

            var totalItems = await query.CountAsync();

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalItems);
        }
    }

}
