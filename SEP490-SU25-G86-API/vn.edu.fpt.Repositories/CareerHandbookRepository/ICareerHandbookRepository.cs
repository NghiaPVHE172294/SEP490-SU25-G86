using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CareerHandbookRepository
{
    public interface ICareerHandbookRepository
    {
        Task<List<CareerHandbook>> GetAllAsync(bool includeDeleted = false);
        Task<List<CareerHandbook>> GetAllPublishedAsync();
        Task<CareerHandbook?> GetByIdAsync(int id);
        Task<CareerHandbook?> GetBySlugAsync(string slug);
        Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null);
        Task AddAsync(CareerHandbook handbook);
        Task UpdateAsync(CareerHandbook handbook);
        Task SoftDeleteAsync(int id); // thêm hàm delete mềm
    }
}
