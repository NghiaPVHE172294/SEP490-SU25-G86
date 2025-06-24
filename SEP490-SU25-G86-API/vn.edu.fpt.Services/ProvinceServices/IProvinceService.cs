using SEP490_SU25_G86_API.vn.edu.fpt.DTO.ProvinceDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.ProvinceDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.ProvinceServices
{
    public interface IProvinceService
    {
        Task<IEnumerable<ProvinceGetDTO>> GetAllAsync();
        Task<int> AddAsync(AddProvinceDTO dto);

    }
}
