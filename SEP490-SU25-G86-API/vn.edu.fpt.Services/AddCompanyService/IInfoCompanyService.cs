using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AddCompanyService
{
    public interface IInfoCompanyService
    {
        Task<CompanyDetailDTO?> GetCompanyByAccountIdAsync(int accountId);
        Task<CompanyDetailDTO?> GetCompanyByIdAsync(int companyId);
        Task<bool> CreateCompanyAsync(int accountId, CompanyCreateUpdateDTO dto);
        Task<bool> UpdateCompanyAsync(int companyId, CompanyCreateUpdateDTO dto);
    }
}
