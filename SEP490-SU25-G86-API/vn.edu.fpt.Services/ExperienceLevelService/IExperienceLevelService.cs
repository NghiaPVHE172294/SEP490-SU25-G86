using SEP490_SU25_G86_API.vn.edu.fpt.DTO.ExperienceLevelDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.ExperienceLevelService
{
    public interface IExperienceLevelService
    {
        Task<List<ExperienceLevelDTO>> GetAllAsync();
        Task<int> AddAsync(AddExperienceLevelDTO dto);
    }
} 