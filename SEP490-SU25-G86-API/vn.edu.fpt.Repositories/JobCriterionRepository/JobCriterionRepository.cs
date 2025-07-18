using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobCriterionRepository
{
    public class JobCriterionRepository : IJobCriterionRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public JobCriterionRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<List<JobCriterion>> GetJobCriteriaByUserIdAsync(int userId)
        {
            return await _context.JobCriteria
                .Where(jc => jc.CreatedByUserId == userId && !jc.IsDelete)
                .ToListAsync();
        }

        public async Task<JobCriterion> AddJobCriterionAsync(JobCriterion jobCriterion)
        {
            _context.JobCriteria.Add(jobCriterion);
            await _context.SaveChangesAsync();
            return jobCriterion;
        }

        public async Task<JobCriterion> UpdateJobCriterionAsync(JobCriterion jobCriterion)
        {
            _context.JobCriteria.Update(jobCriterion);
            await _context.SaveChangesAsync();
            return jobCriterion;
        }
    }
} 