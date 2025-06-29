using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AddCompanyRepository
{
    public interface IAddCompanyRepository
    {
        Task<Company> AddCompanyAsync(Company company);
    }
}
