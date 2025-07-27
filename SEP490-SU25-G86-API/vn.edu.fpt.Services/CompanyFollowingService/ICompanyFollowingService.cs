using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyFollowingService
{
    public interface ICompanyFollowingService
    {
        Task<IEnumerable<CompanyFollowingDTO>> GetFollowedCompaniesAsync(int userId, int page, int pageSize);
        Task<int> CountFollowedCompaniesAsync(int userId);
        Task<IEnumerable<CompanyFollowingDTO>> GetSuggestedCompaniesAsync(int userId, int page, int pageSize);
        Task<int> CountSuggestedCompaniesAsync(int userId);
        Task<bool> UnfollowCompanyAsync(int followId); // thêm mới

    }
}
