using SEP490_SU25_G86_API.vn.edu.fpt.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.IRepositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
    }
}
