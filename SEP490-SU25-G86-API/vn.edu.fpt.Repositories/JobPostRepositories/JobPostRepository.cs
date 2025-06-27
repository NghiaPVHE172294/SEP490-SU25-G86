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
                .Include(j => j.Employer)
                .ThenInclude(u => u.Company)
                .Include(j => j.SalaryRange)
                .Include(j => j.Province)
                .Include(j => j.EmploymentType)
                .Include(j => j.ExperienceLevel)
                .Include(j => j.Industry)
                .Include(j => j.JobLevel)
                .Where(j => j.EmployerId == employerId)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<JobPost?> GetJobPostByIdAsync(int jobPostId)
        {
            return await _context.JobPosts
                .Include(j => j.Employer)
                .ThenInclude(u => u.Company)
                .Include(j => j.Industry)
                .Include(j => j.JobPosition)
                .Include(j => j.SalaryRange)
                .Include(j => j.Province)
                .Include(j => j.ExperienceLevel)
                .Include(j => j.JobLevel)
                .Include(j => j.EmploymentType)
                .FirstOrDefaultAsync(j => j.JobPostId == jobPostId);
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

        public async Task<JobPost> AddJobPostAsync(JobPost jobPost)
        {
            _context.JobPosts.Add(jobPost);
            await _context.SaveChangesAsync();
            return jobPost;
        }

        public async Task<Industry> AddIndustryIfNotExistsAsync(string industryName)
        {
            var industry = await _context.Industries.FirstOrDefaultAsync(i => i.IndustryName == industryName);
            if (industry == null)
            {
                industry = new Industry { IndustryName = industryName };
                _context.Industries.Add(industry);
                await _context.SaveChangesAsync();
            }
            return industry;
        }

        public async Task<JobPosition> AddJobPositionIfNotExistsAsync(string jobPositionName, int? industryId = null)
        {
            var jobPosition = await _context.JobPositions.FirstOrDefaultAsync(jp => jp.PostitionName == jobPositionName && jp.IndustryId == industryId);
            if (jobPosition == null)
            {
                jobPosition = new JobPosition { PostitionName = jobPositionName, IndustryId = industryId };
                _context.JobPositions.Add(jobPosition);
                await _context.SaveChangesAsync();
            }
            return jobPosition;
        }

        public async Task<SalaryRange> AddSalaryRangeIfNotExistsAsync(int minSalary, int maxSalary, string currency)
        {
            var salaryRange = await _context.SalaryRanges.FirstOrDefaultAsync(sr => sr.MinSalary == minSalary && sr.MaxSalary == maxSalary && sr.Currency == currency);
            if (salaryRange == null)
            {
                salaryRange = new SalaryRange { MinSalary = minSalary, MaxSalary = maxSalary, Currency = currency };
                _context.SalaryRanges.Add(salaryRange);
                await _context.SaveChangesAsync();
            }
            return salaryRange;
        }

        public async Task<Province> AddProvinceIfNotExistsAsync(string provinceName)
        {
            var province = await _context.Provinces.FirstOrDefaultAsync(p => p.ProvinceName == provinceName);
            if (province == null)
            {
                province = new Province { ProvinceName = provinceName };
                _context.Provinces.Add(province);
                await _context.SaveChangesAsync();
            }
            return province;
        }

        public async Task<ExperienceLevel> AddExperienceLevelIfNotExistsAsync(string experienceLevelName)
        {
            var exp = await _context.ExperienceLevels.FirstOrDefaultAsync(e => e.ExperienceLevelName == experienceLevelName);
            if (exp == null)
            {
                exp = new ExperienceLevel { ExperienceLevelName = experienceLevelName };
                _context.ExperienceLevels.Add(exp);
                await _context.SaveChangesAsync();
            }
            return exp;
        }

        public async Task<JobLevel> AddJobLevelIfNotExistsAsync(string jobLevelName)
        {
            var jl = await _context.JobLevels.FirstOrDefaultAsync(j => j.JobLevelName == jobLevelName);
            if (jl == null)
            {
                jl = new JobLevel { JobLevelName = jobLevelName };
                _context.JobLevels.Add(jl);
                await _context.SaveChangesAsync();
            }
            return jl;
        }

        public async Task<EmploymentType> AddEmploymentTypeIfNotExistsAsync(string employmentTypeName)
        {
            var et = await _context.EmploymentTypes.FirstOrDefaultAsync(e => e.EmploymentTypeName == employmentTypeName);
            if (et == null)
            {
                et = new EmploymentType { EmploymentTypeName = employmentTypeName };
                _context.EmploymentTypes.Add(et);
                await _context.SaveChangesAsync();
            }
            return et;
        }
    }

}
