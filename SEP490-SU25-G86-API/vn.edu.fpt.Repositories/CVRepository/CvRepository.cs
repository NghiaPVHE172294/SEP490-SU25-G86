using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository
{
    public class CvRepository : ICvRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public CvRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<List<Cv>> GetAllByUserAsync(int userId)
        {
            return await _context.Cvs.Where(c => c.UploadByUserId == userId && !c.IsDelete).OrderByDescending(c => c.UploadDate).ToListAsync();
        }

        public async Task<Cv?> GetByIdAsync(int cvId)
        {
            return await _context.Cvs.FirstOrDefaultAsync(c => c.CvId == cvId && !c.IsDelete);
        }

        public async Task AddAsync(Cv cv)
        {
            _context.Cvs.Add(cv);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cv cv)
        {
            _context.Cvs.Remove(cv);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountByUserAsync(int userId)
        {
            return await _context.Cvs.CountAsync(c => c.UploadByUserId == userId && !c.IsDelete);
        }
    }
}