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

        public async Task<(List<CompanyListDTO> Companies, int TotalCount)> GetPagedCompanyListWithJobPostCountAsync(int page, int pageSize)
        {
            var (companies, totalCount) = await _repo.GetPagedCompaniesAsync(page, pageSize);

            var result = companies.Select(company => new CompanyListDTO
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
                FollowerCount = company.CompanyFollowers.Count,

                TotalJobPostEnabled = company.Users
            .SelectMany(u => u.JobPosts)
            .Count(jp => jp.IsDelete == false && jp.Status == "OPEN")
            }).ToList();

            return (result, totalCount);
        }
        public async Task<List<CompanyLogoDTO>> GetAllCompanyLogos()
        {
            var companies = await _repo.GetAllCompanies();

            return companies.Select(c => new CompanyLogoDTO
            {
                Id = c.CompanyId,
                LogoUrl = c.LogoUrl
            }).ToList();
        }

        public async Task<CompanyDTO?> GetLatestCompanyDtoAsync()
        {
            var company = await _repo.GetLatestCompanyAsync();
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
                IndustryName = company.Industry?.IndustryName,
                FollowersCount = company.CompanyFollowers?.Count ?? 0
            };
        }
    }
}
