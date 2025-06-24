using SEP490_SU25_G86_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ExperienceLevelRepository
{
    public class ExperienceLevelRepository : IExperienceLevelRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public ExperienceLevelRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }
        public async Task<List<ExperienceLevel>> GetAllAsync()
        {
            return await _context.ExperienceLevels.ToListAsync();
        }
        public void Add(ExperienceLevel entity)
        {
            _context.ExperienceLevels.Add(entity);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
} 