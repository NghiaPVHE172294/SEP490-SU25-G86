using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SavedJobRepositories
{
    public interface ISavedJobRepository
    {
        Task<IEnumerable<SavedJob>> GetByUserIdAsync(int userId);
        Task<SavedJob?> GetByUserAndJobPostAsync(int userId, int jobPostId);
        Task CreateAsync(SavedJob savedJob);
        Task<bool> DeleteAsync(int saveJobId);
    }
}
