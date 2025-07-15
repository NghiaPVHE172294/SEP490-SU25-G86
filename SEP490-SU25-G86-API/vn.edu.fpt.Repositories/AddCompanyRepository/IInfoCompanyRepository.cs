using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AddCompanyRepository
{
    public interface IInfoCompanyRepository
    {
        Task<Company?> GetByUserIdAsync(int userId);
        Task<Company?> GetByIdAsync(int companyId);
        Task CreateAsync(Company company);
        Task UpdateAsync(Company company);
    }
}
