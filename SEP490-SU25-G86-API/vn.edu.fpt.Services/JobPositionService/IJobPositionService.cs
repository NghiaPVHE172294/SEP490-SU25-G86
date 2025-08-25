using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPositionDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPositionService
{
    public interface IJobPositionService
    {
        Task<List<JobPositionDTO>> GetAllAsync();
        Task<List<JobPositionDTO>> GetByIndustryIdAsync(int industryId);
        Task<int> AddAsync(AddJobPositionDTO dto);
    }
}