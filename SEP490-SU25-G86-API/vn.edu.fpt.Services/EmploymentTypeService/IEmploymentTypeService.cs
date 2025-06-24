using SEP490_SU25_G86_API.vn.edu.fpt.DTO.EmploymentTypeDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.EmploymentTypeService
{
    public interface IEmploymentTypeService
    {
        Task<List<EmploymentTypeDTO>> GetAllAsync();
        Task<int> AddAsync(AddEmploymentTypeDTO dto);
    }
} 