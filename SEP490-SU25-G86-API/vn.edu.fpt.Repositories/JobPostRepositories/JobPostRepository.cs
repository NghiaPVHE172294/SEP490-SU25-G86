using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPostRepositories
{
    public class JobPostRepository : IJobPostRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public JobPostRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<JobPost>> GetAllAsync()
        {
            return await _context.JobPosts
                .AsNoTracking()
                .ToListAsync();
        }
    }

}
