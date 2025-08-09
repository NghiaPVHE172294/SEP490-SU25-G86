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

        // Lấy tất cả (mặc định chỉ lấy chưa xóa)
        public async Task<List<CareerHandbook>> GetAllAsync(bool includeDeleted = false)
        {
            var query = _context.CareerHandbooks
                .Include(h => h.Category)
                .AsQueryable();

            if (!includeDeleted)
                query = query.Where(h => !h.IsDeleted);

            return await query.OrderByDescending(h => h.CreatedAt).ToListAsync();
        }

        // Lấy tất cả bản đã publish (chỉ lấy chưa xóa)
        public async Task<List<CareerHandbook>> GetAllPublishedAsync()
        {
            return await _context.CareerHandbooks
                .Include(h => h.Category)
                .Where(h => h.IsPublished && !h.IsDeleted)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        // Lấy chi tiết theo Id (mặc định không lấy bản đã xóa)
        public async Task<CareerHandbook?> GetByIdAsync(int id)
        {
            return await _context.CareerHandbooks
                .Include(h => h.Category)
                .FirstOrDefaultAsync(h => h.HandbookId == id && !h.IsDeleted);
        }

        // Lấy chi tiết theo Slug (chỉ lấy bản đã publish và chưa xóa)
        public async Task<CareerHandbook?> GetBySlugAsync(string slug)
        {
            return await _context.CareerHandbooks
                .Include(h => h.Category)
                .FirstOrDefaultAsync(h => h.Slug == slug && h.IsPublished && !h.IsDeleted);
        }

        // Kiểm tra slug tồn tại (chỉ kiểm tra với bản chưa xóa)
        public async Task<bool> ExistsBySlugAsync(string slug, int? excludeId = null)
        {
            return await _context.CareerHandbooks
                .AnyAsync(h => h.Slug == slug && !h.IsDeleted && (!excludeId.HasValue || h.HandbookId != excludeId.Value));
        }

        // Thêm mới
        public async Task AddAsync(CareerHandbook handbook)
        {
            _context.CareerHandbooks.Add(handbook);
            await _context.SaveChangesAsync();
        }

        // Cập nhật
        public async Task UpdateAsync(CareerHandbook handbook)
        {
            _context.CareerHandbooks.Update(handbook);
            await _context.SaveChangesAsync();
        }

        // Xóa mềm
        public async Task SoftDeleteAsync(int id)
        {
            var handbook = await _context.CareerHandbooks.FindAsync(id);
            if (handbook != null && !handbook.IsDeleted)
            {
                handbook.IsDeleted = true;
                handbook.UpdatedAt = DateTime.UtcNow;
                _context.CareerHandbooks.Update(handbook);
                await _context.SaveChangesAsync();
            }
        }
    }
}
