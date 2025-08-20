using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository
{
    public interface ICvRepository
    {
        Task<List<Cv>> GetAllByUserAsync(int userId);
        Task<Cv?> GetByIdAsync(int cvId);
        Task<int> AddAsync(Cv cv);
        Task DeleteAsync(Cv cv);
        Task<int> CountByUserAsync(int userId);
        Task UpdateAsync(Cv cv);
        Task<bool> HasBeenUsedInSubmissionAsync(int cvId);
    }
}