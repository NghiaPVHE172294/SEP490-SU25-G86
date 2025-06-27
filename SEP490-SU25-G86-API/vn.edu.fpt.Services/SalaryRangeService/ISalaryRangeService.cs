using SEP490_SU25_G86_API.vn.edu.fpt.DTO.SalaryRangeDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.SalaryRangeService
{
    public interface ISalaryRangeService
    {
        Task<List<SalaryRangeDTO>> GetAllAsync();
        Task<int> AddAsync(AddSalaryRangeDTO dto);
    }
} 