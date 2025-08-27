using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Helpers;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SynonymService;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories
{
    public class JobPostRepository : IJobPostRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        private readonly ISynonymService _synonymService;
        private readonly AutoMapper.IConfigurationProvider _mapperCfg;
        public JobPostRepository(SEP490_G86_CvMatchContext context, ISynonymService synonymService, IMapper mapper)
        {
            _context = context;
            _synonymService = synonymService;
            _mapperCfg = mapper.ConfigurationProvider;
        }

        public async Task<(IEnumerable<JobPost> Posts, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null, int? salaryRangeId = null, int? experienceLevelId = null, int? candidateId = null)
        {
            var query = _context.JobPosts
        .Include(j => j.Employer)
        .ThenInclude(u => u.Company)
        .Include(j => j.SalaryRange)
        .Include(j => j.Province)
        .Where(j => j.Status == "OPEN" &&
                    (!j.EndDate.HasValue || j.EndDate.Value.Date >= DateTime.UtcNow.Date))
        .OrderByDescending(j => j.CreatedDate)
        .AsQueryable();

            // Filter Region
            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(j =>
                    j.Province != null &&
                    j.Province.Region != null &&
                    EF.Functions.Like(j.Province.Region, $"%{region}%"));
            }

            // Filter SalaryRangeId
            if (salaryRangeId.HasValue)
            {
                query = query.Where(j => j.SalaryRangeId == salaryRangeId.Value);
            }

            // Filter ExperienceLevelId
            if (experienceLevelId.HasValue)
            {
                query = query.Where(j => j.ExperienceLevelId == experienceLevelId.Value);
            }
            // Lọc bỏ job posts từ blocked companies nếu có candidateId
            if (candidateId.HasValue)
            {
                var blockedCompanyIds = await _context.BlockedCompanies
                    .Where(bc => bc.CandidateId == candidateId.Value)
                    .Select(bc => bc.CompanyId)
                    .ToListAsync();

                if (blockedCompanyIds.Any())
                {
                    query = query.Where(j => j.Employer == null
                                          || j.Employer.CompanyId == null
                                          || !blockedCompanyIds.Contains(j.Employer.CompanyId.Value));
                }
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
            List<int>? datePostedRanges = null,
            string? keyword = null,
            int? candidateId = null)
        {
            var query = _context.JobPosts
                .Include(j => j.Employer).ThenInclude(e => e.Company)
                .Include(j => j.Province)
                .Include(j => j.EmploymentType)
                .Include(j => j.ExperienceLevel)
                .Include(j => j.Industry)
                .Include(j => j.JobLevel)
                .Include(j => j.SalaryRange)
                .Where(j =>
                        j.Status == "OPEN" &&
                        (!j.EndDate.HasValue || j.EndDate.Value.Date >= DateTime.UtcNow.Date) &&
                        !j.IsDelete)
                .OrderByDescending(j => j.CreatedDate)
                .AsQueryable();
            //Lọc bỏ job posts từ blocked companies nếu có candidateId
            if (candidateId.HasValue)
            {
                var blockedCompanyIds = await _context.BlockedCompanies
                    .Where(bc => bc.CandidateId == candidateId.Value)
                    .Select(bc => bc.CompanyId)
                    .ToListAsync();

                if (blockedCompanyIds.Any())
                {
                    query = query.Where(j =>
                        j.Employer != null &&
                        j.Employer.CompanyId.HasValue &&
                        !blockedCompanyIds.Contains(j.Employer.CompanyId.Value));
                }
            }
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

            // Lọc theo keyword
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var terms = _synonymService.ExpandKeywords(keyword);
                //Dynamic Predicate
                Expression<Func<JobPost, bool>> predicate = j => false; 
                foreach (var term in terms)
                {
                    var pattern = $"%{term}%";
                    predicate = predicate.Or(j =>
                        EF.Functions.Like(j.Title, pattern) ||
                        (j.Employer != null && EF.Functions.Like(j.Employer.FullName, pattern)));
                }
                query = query.Where(predicate);
            }

            var total = await query.CountAsync();

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, total);
        }

        public async Task<JobPost> AddJobPostAsync(JobPost jobPost)
        {
            _context.JobPosts.Add(jobPost);
            await _context.SaveChangesAsync();
            return jobPost;
        }

        public async Task<JobPost> UpdateJobPostAsync(JobPost jobPost)
        {
            _context.JobPosts.Update(jobPost);
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

        public async Task<(IEnumerable<JobPost> Posts, int TotalItems)> GetJobPostsByCompanyIdAsync(int companyId, int page, int pageSize)
        {
            var query = _context.JobPosts
                .Include(j => j.Employer).ThenInclude(u => u.Company)
                .Include(j => j.SalaryRange)
                .Include(j => j.Province)
                .Include(j => j.EmploymentType)
                .Include(j => j.ExperienceLevel)
                .Include(j => j.Industry)
                .Include(j => j.JobLevel)
                .Where(j => j.Employer != null && j.Employer.CompanyId == companyId &&
                            j.Status == "OPEN" &&
                            (!j.EndDate.HasValue || j.EndDate.Value.Date >= DateTime.UtcNow.Date))
                .AsNoTracking()
                .OrderByDescending(j => j.CreatedDate);

            var totalItems = await query.CountAsync();

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalItems);
        }

        public async Task<List<Cvsubmission>> GetCvSubmissionsByJobPostIdAsync(int jobPostId)
        {
            return await _context.Cvsubmissions
                .Include(s => s.Cv)
                .Include(s => s.SubmittedByUser)
                .Include(s => s.MatchedCvandJobPost) // Thêm dòng này để lấy TotalScore
                .Where(s => s.JobPostId == jobPostId && !s.IsDelete)
                .ToListAsync();
        }

        public async Task<List<int>> GetAppliedJobPostIdsAsync(int candidateId)
        {
            return await _context.Cvsubmissions
                .Where(s => s.SubmittedByUserId == candidateId && !s.IsDelete)
                .Select(s => s.JobPostId ?? 0)
                .Where(id => id != 0)
                .ToListAsync();
        }

        public async Task<bool> SoftDeleteAsync(int jobPostId, int? employerId = null)
        {
            var query = _context.JobPosts.IgnoreQueryFilters().AsQueryable();

            if (employerId.HasValue)
                query = query.Where(j => j.EmployerId == employerId.Value);

            var jobPost = await query.FirstOrDefaultAsync(j => j.JobPostId == jobPostId);
            if (jobPost == null) return false;
            if (jobPost.IsDelete) return true; // đã xóa mềm trước đó

            jobPost.IsDelete = true;
            jobPost.UpdatedDate = DateTime.UtcNow;

            _context.JobPosts.Update(jobPost);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestoreAsync(int jobPostId, int? employerId = null)
        {
            var query = _context.JobPosts.IgnoreQueryFilters().AsQueryable();

            if (employerId.HasValue)
                query = query.Where(j => j.EmployerId == employerId.Value);

            var jobPost = await query.FirstOrDefaultAsync(j => j.JobPostId == jobPostId);
            if (jobPost == null) return false;
            if (!jobPost.IsDelete) return true; // chưa bị xóa mềm

            jobPost.IsDelete = false;
            jobPost.UpdatedDate = DateTime.UtcNow;

            _context.JobPosts.Update(jobPost);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<RelatedJobItemDTO>> GetRelatedByIndustryAsync(
            int industryId, int take = 5, int? excludeJobPostId = null, CancellationToken ct = default)
        {
            var q = _context.JobPosts
                .AsNoTracking()
                .Where(j => j.IndustryId == industryId);

            if (excludeJobPostId.HasValue)
                q = q.Where(j => j.JobPostId != excludeJobPostId.Value);

            return await q
                .OrderByDescending(j => j.CreatedDate)
                .ProjectTo<RelatedJobItemDTO>(_mapperCfg) // <- JOIN + select đúng cột
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
