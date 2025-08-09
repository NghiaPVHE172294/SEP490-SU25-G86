using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobCriterionRepository
{
    public interface IJobCriterionRepository
    {
        Task<List<JobCriterion>> GetJobCriteriaByUserIdAsync(int userId);
        Task<JobCriterion> AddJobCriterionAsync(JobCriterion jobCriterion);
        Task<JobCriterion> UpdateJobCriterionAsync(JobCriterion jobCriterion);
        Task<bool> SoftDeleteAsync(int id);
        Task<bool> RestoreAsync(int id);
    }
} 