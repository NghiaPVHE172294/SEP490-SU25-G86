using SEP490_SU25_G86_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPositionRepository
{
    public class JobPositionRepository : IJobPositionRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public JobPositionRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }
        public async Task<List<JobPosition>> GetAllAsync()
        {
            return await _context.JobPositions.ToListAsync();
        }
        public void Add(JobPosition entity)
        {
            _context.JobPositions.Add(entity);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<List<JobPosition>> GetByIndustryIdAsync(int industryId)
        {
            return await _context.JobPositions
                .Where(jp => jp.IndustryId == industryId)
                .ToListAsync();
        }
    }
} 