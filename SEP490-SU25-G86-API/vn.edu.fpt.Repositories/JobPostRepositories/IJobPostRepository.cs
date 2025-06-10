using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories
{
    public interface IJobPostRepository
    {
        Task<IEnumerable<JobPost>> GetAllAsync();
        Task<IEnumerable<JobPost>> GetByEmployerIdAsync(int employerId);

    }

}
