using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AddCompanyService
{
    public interface IAddCompanyService
    {
        Task<Company> AddCompanyAsync(AddCompanyDTO dto, int employerId);
    }
}
