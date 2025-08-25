using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository;

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
                    .ThenInclude(jp => jp.Employer)
                        .ThenInclude(e => e.Company)     // đi qua Employer để lấy Company
                .Include(s => s.JobPost)
                    .ThenInclude(jp => jp.SalaryRange)   // lấy salary range
                .Where(s => s.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }



        public async Task<SavedJob?> GetByUserAndJobPostAsync(int userId, int jobPostId)
        {
            return await _context.SavedJobs
                .FirstOrDefaultAsync(x => x.UserId == userId && x.JobPostId == jobPostId);
        }

        public async Task CreateAsync(SavedJob savedJob)
        {
            _context.SavedJobs.Add(savedJob);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int saveJobId)
        {
            var job = await _context.SavedJobs.FindAsync(saveJobId);
            if (job == null) return false;

            _context.SavedJobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
