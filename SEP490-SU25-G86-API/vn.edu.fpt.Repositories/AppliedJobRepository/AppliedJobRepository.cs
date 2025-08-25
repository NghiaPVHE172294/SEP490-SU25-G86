using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AppliedJobRepository
{
    public class AppliedJobRepository : IAppliedJobRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public AppliedJobRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cvsubmission>> GetByUserIdAsync(int userId)
        {
            return await _context.Cvsubmissions
                .Include(s => s.JobPost)
                    .ThenInclude(jp => jp.Employer)
                        .ThenInclude(e => e.Company)
                .Include(s => s.JobPost)
                    .ThenInclude(jp => jp.SalaryRange)
                .Include(s => s.Cv)
                .Where(s => s.SubmittedByUserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasUserAppliedToJobAsync(int userId, int jobPostId)
        {
            return await _context.Cvsubmissions.AnyAsync(s => s.SubmittedByUserId == userId && s.JobPostId == jobPostId && !s.IsDelete);
        }
    }
} 