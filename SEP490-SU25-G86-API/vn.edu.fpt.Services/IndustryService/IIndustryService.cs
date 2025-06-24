using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.IndustryDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.IndustryService
{
    public interface IIndustryService
    {
        Task<IEnumerable<Industry>> GetAllIndustriesAsync();
        Task<int> AddAsync(AddIndustryDTO dto);
    }
}
