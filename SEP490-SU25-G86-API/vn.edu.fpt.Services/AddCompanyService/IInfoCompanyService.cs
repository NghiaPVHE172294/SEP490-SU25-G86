using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AddCompanyService
{
    public interface IInfoCompanyService
    {
        Task<CompanyDetailDTO?> GetCompanyByUserIdAsync(int userId);
        Task<CompanyDetailDTO?> GetCompanyByIdAsync(int companyId);
        Task<bool> CreateCompanyAsync(int userId, CompanyCreateUpdateDTO dto);
        Task<bool> UpdateCompanyAsync(int companyId, CompanyCreateUpdateDTO dto);
    }
}
