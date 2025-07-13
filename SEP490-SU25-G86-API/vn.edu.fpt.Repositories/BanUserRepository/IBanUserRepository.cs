using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BanUserRepository
{
    public interface IBanUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task UpdateAsync(User user);
    }
}
