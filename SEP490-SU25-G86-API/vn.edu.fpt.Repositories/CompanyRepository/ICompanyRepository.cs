using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyRepository
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id);

        Task<(List<Company> Companies, int TotalCount)> GetPagedCompaniesAsync(int page, int pageSize);
        Task<List<Company>> GetAllCompanies();
        Task<Company?> GetLatestCompanyAsync();
    }
}
