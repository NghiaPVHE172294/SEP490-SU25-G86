using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyFollowingRepositories;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyFollowingService
{
    public class CompanyFollowingService : ICompanyFollowingService
    {
        private readonly ICompanyFollowingRepository _repository;

        public CompanyFollowingService(ICompanyFollowingRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CompanyFollowingDTO>> GetFollowedCompaniesAsync(int userId)
        {
            return await _repository.GetFollowedCompaniesByUserIdAsync(userId);
        }
    }
}
