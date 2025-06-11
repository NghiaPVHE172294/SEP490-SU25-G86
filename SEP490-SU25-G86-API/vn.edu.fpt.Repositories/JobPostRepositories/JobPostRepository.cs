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

        public async Task<(IEnumerable<JobPost> Posts, int TotalItems)> GetPagedJobPostsAsync(int page, int pageSize, string? region = null)
        {
            var query = _context.JobPosts
        .Include(j => j.Employer)
        .Include(j => j.SalaryRange)
        .Include(j => j.Province)
        .OrderByDescending(j => j.CreatedDate)
        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(region))
            {
                query = query.Where(j =>
                    j.Province != null &&
                    j.Province.Region != null &&
                    EF.Functions.Like(j.Province.Region, $"%{region}%"));
            }

            var totalItems = await query.CountAsync();

            var posts = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (posts, totalItems);
        }

        public async Task<IEnumerable<JobPost>> GetAllAsync()
        {
            return await _context.JobPosts
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<JobPost>> GetByEmployerIdAsync(int employerId)
        {
            return await _context.JobPosts
                .Where(j => j.EmployerId == employerId)
                .AsNoTracking()
                .ToListAsync();
        }

    }

}
