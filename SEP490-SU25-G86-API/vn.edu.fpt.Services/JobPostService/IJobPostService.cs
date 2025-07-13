using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService
{
    public interface IJobPostService
    {
        Task<(IEnumerable<JobPostHomeDto>, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null, int? candidateId = null);

        Task<IEnumerable<JobPostDTO>> GetAllJobPostsAsync();

        Task<IEnumerable<JobPostListDTO>> GetByEmployerIdAsync(int employerId);


        Task<ViewDetailJobPostDTO?> GetJobPostDetailByIdAsync(int jobPostId);


        Task<(IEnumerable<JobPostListDTO> Posts, int TotalItems)> GetFilteredJobPostsAsync(
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
            int? candidateId = null
        );

        Task<ViewDetailJobPostDTO> AddJobPostAsync(AddJobPostDTO dto, int employerId);

        Task<(IEnumerable<JobPostListDTO> Posts, int TotalItems)> GetJobPostsByCompanyIdAsync(int companyId, int page, int pageSize);

        Task<List<CvSubmissionForJobPostDTO>> GetCvSubmissionsByJobPostIdAsync(int jobPostId);

    }

}
