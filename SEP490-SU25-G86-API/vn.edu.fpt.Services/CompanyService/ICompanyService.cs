using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyService
{
    public interface ICompanyService
    {
        Task<CompanyDTO?> GetCompanyDtoByIdAsync(int id);
    }
}
