using SEP490_SU25_G86_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.EmploymentTypeRepository
{
    public class EmploymentTypeRepository : IEmploymentTypeRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public EmploymentTypeRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }
        public async Task<List<EmploymentType>> GetAllAsync()
        {
            return await _context.EmploymentTypes.ToListAsync();
        }
        public void Add(EmploymentType entity)
        {
            _context.EmploymentTypes.Add(entity);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
} 