using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.PhoneOtpRepository
{
    public interface IPhoneOtpRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task UpdateAsync(User user);
    }
}
