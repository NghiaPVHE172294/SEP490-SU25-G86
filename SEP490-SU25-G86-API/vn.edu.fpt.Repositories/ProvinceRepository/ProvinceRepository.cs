using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ProvinceRepository
{
    public class ProvinceRepository : IProvinceRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public ProvinceRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<List<Province>> GetAllAsync()
        {
            return await _context.Provinces.ToListAsync();
        }

        public void Add(Province entity)
        {
            _context.Provinces.Add(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
