using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobLevelRepository
{
    public interface IJobLevelRepository
    {
        Task<List<JobLevel>> GetAllAsync();
        void Add(JobLevel entity);
        Task<int> SaveChangesAsync();
    }
} 