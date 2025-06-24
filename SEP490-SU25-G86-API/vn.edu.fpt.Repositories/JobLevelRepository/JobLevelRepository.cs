using SEP490_SU25_G86_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobLevelRepository
{
    public class JobLevelRepository : IJobLevelRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public JobLevelRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }
        public async Task<List<JobLevel>> GetAllAsync()
        {
            return await _context.JobLevels.ToListAsync();
        }
        public void Add(JobLevel entity)
        {
            _context.JobLevels.Add(entity);
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
} 