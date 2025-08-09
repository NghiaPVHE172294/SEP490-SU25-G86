using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobCriterionService
{
    public interface IJobCriterionService
    {
        Task<List<JobCriterionDTO>> GetJobCriteriaByUserIdAsync(int userId);

        Task<JobCriterionDTO> AddJobCriterionAsync(AddJobCriterionDTO dto, int userId);
        Task<JobCriterionDTO> UpdateJobCriterionAsync(UpdateJobCriterionDTO dto, int userId);
        Task<bool> SoftDeleteJobCriterionAsync(int id, int userId);  // Xóa mềm
        Task<bool> RestoreJobCriterionAsync(int id, int userId);      // Khôi phục
    }
} 