using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SavedJobRepositories
{
    public interface ISavedJobRepository
    {
        Task<IEnumerable<SavedJob>> GetByUserIdAsync(int userId);
    }
}
