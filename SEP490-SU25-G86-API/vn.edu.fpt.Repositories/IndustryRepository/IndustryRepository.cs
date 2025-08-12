using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.IndustryRepository
{
    public class IndustryRepository : IIndustryRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public IndustryRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Industry>> GetAllAsync()
        {
            return await _context.Industries.ToListAsync();
        }

        public void Add(Industry entity)
        {
            _context.Industries.Add(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<(Industry Industry, int JobPostCount)>> GetIndustriesWithJobPostCount(int page, int pageSize)
        {
            var data = await _context.Industries
        .OrderBy(ind => ind.IndustryName) 
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(ind => new
        {
            Industry = ind,
            JobPostCount = ind.JobPosts.Count(j => j.Status == "OPEN" && j.IsDelete == false &&
                    (!j.EndDate.HasValue || j.EndDate.Value.Date >= DateTime.UtcNow.Date))
        })
        .ToListAsync();

            return data.Select(x => (x.Industry, x.JobPostCount));
        }
    }
}
