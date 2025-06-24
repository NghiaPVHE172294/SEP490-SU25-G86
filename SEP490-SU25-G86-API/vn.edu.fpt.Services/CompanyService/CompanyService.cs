using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyService
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repo;
        public CompanyService(ICompanyRepository repo)
        {
            _repo = repo;
        }

        public async Task<CompanyDTO?> GetCompanyDtoByIdAsync(int id)
        {
            var company = await _repo.GetByIdAsync(id);
            if (company == null) return null;

            return new CompanyDTO
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                Website = company.Website,
                CompanySize = company.CompanySize,
                Email = company.Email,
                Phone = company.Phone,
                Address = company.Address,
                Description = company.Description,
                LogoUrl = company.LogoUrl,
                IndustryName = company.Industry.IndustryName,
                FollowersCount = company.CompanyFollowers.Count
            };
        }
    }
}
