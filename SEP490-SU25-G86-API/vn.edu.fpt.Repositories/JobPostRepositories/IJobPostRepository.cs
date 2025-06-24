using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories
{
    public interface IJobPostRepository
    {
        Task<(IEnumerable<JobPost> Posts, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null);
        Task<IEnumerable<JobPost>> GetAllAsync();
        Task<IEnumerable<JobPost>> GetByEmployerIdAsync(int employerId);
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
            string? keyword = null
        );
    }
}
