using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetUserByAccountIdAsync(int accountId);
        Task UpdateUserAsync(User user);
        Task<bool> FollowCompanyAsync(int accountId, int companyId);
        Task<bool> BlockCompanyAsync(int accountId, int companyId, string? reason);
        Task<bool> IsCompanyFollowedAsync(int accountId, int companyId);
        Task<bool> IsCompanyBlockedAsync(int accountId, int companyId);
    }
}
