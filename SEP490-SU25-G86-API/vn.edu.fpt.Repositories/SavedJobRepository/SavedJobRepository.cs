using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SavedJobRepositories
{
    public class SavedJobRepository : ISavedJobRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public SavedJobRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SavedJob>> GetByUserIdAsync(int userId)
        {
            return await _context.SavedJobs
                .Include(s => s.JobPost)
                .Where(s => s.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> DeleteAsync(int saveJobId)
        {
            var entity = await _context.SavedJobs.FindAsync(saveJobId);
            if (entity == null)
                return false;
            _context.SavedJobs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
