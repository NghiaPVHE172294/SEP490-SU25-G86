using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyFollowingRepositories
{
    public interface ICompanyFollowingRepository
    {
        Task<IEnumerable<CompanyFollowingDTO>> GetFollowedCompaniesByUserIdAsync(int userId);
    }
}
