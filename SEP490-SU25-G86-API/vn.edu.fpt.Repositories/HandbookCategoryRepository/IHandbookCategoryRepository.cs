using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.HandbookCategoryRepository
{
    public interface IHandbookCategoryRepository
    {
        Task<List<HandbookCategory>> GetAllAsync();
        Task<HandbookCategory?> GetByIdAsync(int id);
        Task AddAsync(HandbookCategory category);
        Task UpdateAsync(HandbookCategory category);
        Task DeleteAsync(HandbookCategory category);
        Task<bool> ExistsByNameAsync(string categoryName, int? excludeId = null);
        Task<bool> HasCareerHandbooksAsync(int categoryId);
    }
}
