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
    }
}
