using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.HandbookCategoryRepository
{
    public class HandbookCategoryRepository : IHandbookCategoryRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public HandbookCategoryRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<List<HandbookCategory>> GetAllAsync()
        {
            return await _context.HandbookCategories
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<HandbookCategory?> GetByIdAsync(int id)
        {
            return await _context.HandbookCategories
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task AddAsync(HandbookCategory category)
        {
            _context.HandbookCategories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(HandbookCategory category)
        {
            _context.HandbookCategories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(HandbookCategory category)
        {
            _context.HandbookCategories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string categoryName, int? excludeId = null)
        {
            return await _context.HandbookCategories
                .AnyAsync(c => c.CategoryName == categoryName &&
                              (!excludeId.HasValue || c.CategoryId != excludeId.Value));
        }

        public async Task<bool> HasCareerHandbooksAsync(int categoryId)
        {
            return await _context.CareerHandbooks
                .AnyAsync(h => h.CategoryId == categoryId);
        }
    }
}
