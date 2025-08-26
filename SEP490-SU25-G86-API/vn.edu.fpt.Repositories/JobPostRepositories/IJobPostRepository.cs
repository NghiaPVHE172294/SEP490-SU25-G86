using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories
{
    public interface IJobPostRepository
    {
        Task<(IEnumerable<JobPost> Posts, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize,
            string? region = null,
            int? salaryRangeId = null,
            int? experienceLevelId = null,
            int? candidateId = null);
        Task<IEnumerable<JobPost>> GetAllAsync();
        Task<IEnumerable<JobPost>> GetByEmployerIdAsync(int employerId);

        Task<JobPost?> GetJobPostByIdAsync(int jobPostId);

        Task<(IEnumerable<JobPost> Posts, int TotalItems)> GetFilteredJobPostsAsync(
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
            int? candidateId = null);

        Task<JobPost> AddJobPostAsync(JobPost jobPost);
        Task<JobPost> UpdateJobPostAsync(JobPost jobPost);
        Task<Industry> AddIndustryIfNotExistsAsync(string industryName);
        Task<JobPosition> AddJobPositionIfNotExistsAsync(string jobPositionName, int? industryId = null);
        Task<SalaryRange> AddSalaryRangeIfNotExistsAsync(int minSalary, int maxSalary, string currency);
        Task<Province> AddProvinceIfNotExistsAsync(string provinceName);
        Task<ExperienceLevel> AddExperienceLevelIfNotExistsAsync(string experienceLevelName);
        Task<JobLevel> AddJobLevelIfNotExistsAsync(string jobLevelName);
        Task<EmploymentType> AddEmploymentTypeIfNotExistsAsync(string employmentTypeName);
        Task<(IEnumerable<JobPost> Posts, int TotalItems)> GetJobPostsByCompanyIdAsync(int companyId, int page, int pageSize);
        Task<List<Cvsubmission>> GetCvSubmissionsByJobPostIdAsync(int jobPostId);
        Task<List<int>> GetAppliedJobPostIdsAsync(int candidateId);
        Task<bool> SoftDeleteAsync(int jobPostId, int? employerId = null);
        Task<bool> RestoreAsync(int jobPostId, int? employerId = null); // (tuỳ chọn)
        Task<List<RelatedJobItemDTO>> GetRelatedByIndustryAsync(
        int industryId, int take = 5, int? excludeJobPostId = null, CancellationToken ct = default);
    }
}
