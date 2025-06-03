using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.vn.edu.fpt.IRepositories;
using SEP490_SU25_G86_API.vn.edu.fpt.Models;
using System;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly Product_TestContext _context;

        public ProductRepository(Product_TestContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
            => await _context.Products.ToListAsync();

        public async Task<Product> GetByIdAsync(int id)
            => await _context.Products.FindAsync(id);

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
