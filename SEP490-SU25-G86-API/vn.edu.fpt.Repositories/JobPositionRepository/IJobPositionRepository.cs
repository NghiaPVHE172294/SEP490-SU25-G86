using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPositionRepository
{
    public interface IJobPositionRepository
    {
        Task<List<JobPosition>> GetAllAsync();
        Task<List<JobPosition>> GetByIndustryIdAsync(int industryId);
        void Add(JobPosition entity);
        Task<int> SaveChangesAsync();
    }
}