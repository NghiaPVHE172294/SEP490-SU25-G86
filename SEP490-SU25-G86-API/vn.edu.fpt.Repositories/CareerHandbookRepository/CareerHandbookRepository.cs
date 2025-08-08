using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CareerHandbookRepository
{

    public class CareerHandbookRepository : ICareerHandbookRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public CareerHandbookRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<List<CareerHandbook>> GetAllAsync(bool includeDeleted = false)
        {
            var query = _context.CareerHandbooks
                .Include(h => h.Category)
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(h => !h.IsDeleted);

            return await query.OrderByDescending(h => h.CreatedAt).ToListAsync();
        }

        public async Task<List<CareerHandbook>> GetAllPublishedAsync()
        {
            return await _context.CareerHandbooks
                .Include(h => h.Category)
                .Where(h => h.IsPublished && !h.IsDeleted)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<CareerHandbook?> GetByIdAsync(int id)
        {
            return await _context.CareerHandbooks
                .Include(h => h.Category)
                .FirstOrDefaultAsync(h => h.HandbookId == id && !h.IsDeleted);
        }

        public async Task<CareerHandbook?> GetBySlugAsync(string slug)
        {
            return await _context.CareerHandbooks
                .Include(h => h.Category)
                .FirstOrDefaultAsync(h => h.Slug == slug && h.IsPublished && !h.IsDeleted);
        }

        public async Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null)
        {
            return await _context.CareerHandbooks
                .AnyAsync(h => h.Slug == slug && (!excludeId.HasValue || h.HandbookId != excludeId.Value));
        }

        public async Task AddAsync(CareerHandbook handbook)
        {
            _context.CareerHandbooks.Add(handbook);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CareerHandbook handbook)
        {
            _context.CareerHandbooks.Update(handbook);
            await _context.SaveChangesAsync();
        }
    }
}
