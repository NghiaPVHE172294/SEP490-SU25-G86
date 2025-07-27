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

        public async Task<IEnumerable<CompanyFollowingDTO>> GetFollowedCompaniesAsync(int userId, int page, int pageSize)
        {
            return await _repository.GetFollowedCompaniesByUserIdAsync(userId, page, pageSize);
        }
        public async Task<int> CountFollowedCompaniesAsync(int userId)
        {
            return await _repository.CountFollowedCompaniesAsync(userId);
        }
        public async Task<IEnumerable<CompanyFollowingDTO>> GetSuggestedCompaniesAsync(int userId, int page, int pageSize)
        {
            return await _repository.GetSuggestedCompaniesAsync(userId, page, pageSize);
        }
        public async Task<int> CountSuggestedCompaniesAsync(int userId)
        {
            return await _repository.CountSuggestedCompaniesAsync(userId);
        }
        public async Task<bool> UnfollowCompanyAsync(int followId)
        {
            return await _repository.UnfollowCompanyAsync(followId);
        }

    }
}
