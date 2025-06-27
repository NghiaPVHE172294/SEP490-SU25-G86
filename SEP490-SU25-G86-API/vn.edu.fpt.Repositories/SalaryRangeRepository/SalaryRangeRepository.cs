using SEP490_SU25_G86_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SalaryRangeRepository
{
    public class SalaryRangeRepository : ISalaryRangeRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public SalaryRangeRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }
        public async Task<List<SalaryRange>> GetAllAsync()
        {
            return await _context.SalaryRanges.Where(s => !s.IsDelete).ToListAsync();
        }
        public async Task AddAsync(SalaryRange entity)
        {
            _context.SalaryRanges.Add(entity);
            await _context.SaveChangesAsync();
        }
    }
} 