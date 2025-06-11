using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService
{
    public interface IJobPostService
    {

        Task<(IEnumerable<JobPostHomeDto>, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null);

        Task<IEnumerable<JobPostDTO>> GetAllJobPostsAsync();

        Task<IEnumerable<JobPostDTO>> GetByEmployerIdAsync(int employerId);

    }

}
