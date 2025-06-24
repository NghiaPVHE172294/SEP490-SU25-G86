using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobLevelDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobLevelService
{
    public interface IJobLevelService
    {
        Task<List<JobLevelDTO>> GetAllAsync();
        Task<int> AddAsync(AddJobLevelDTO dto);
    }
} 