using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AddCompanyRepository
{
    public interface IInfoCompanyRepository
    {
        Task<Company?> GetByAccountIdAsync(int accountId);
        Task<Company?> GetByIdAsync(int companyId);
        Task CreateAsync(Company company);
        Task UpdateAsync(Company company);
        Task<bool> IsDuplicateCompanyAsync(CompanyCreateUpdateDTO dto);

    }
}
