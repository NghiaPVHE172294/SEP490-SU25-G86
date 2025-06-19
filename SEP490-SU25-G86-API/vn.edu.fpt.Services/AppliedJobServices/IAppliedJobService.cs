using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AppliedJobDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices
{
    public interface IAppliedJobService
    {
        Task<IEnumerable<AppliedJobDTO>> GetAppliedJobsByUserIdAsync(int userId);
    }
} 