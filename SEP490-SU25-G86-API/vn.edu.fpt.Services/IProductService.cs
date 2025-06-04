using SEP490_SU25_G86_API.vn.edu.fpt.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
    }
}
