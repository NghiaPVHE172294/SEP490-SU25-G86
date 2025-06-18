using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ProvinceRepositories
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public ProvinceRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Province>> GetAllAsync()
        {
            return await _context.Provinces.ToListAsync();
        }
    }
}
