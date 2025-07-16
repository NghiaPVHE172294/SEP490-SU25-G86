using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BanUserRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.BanUserService
{
    public class BanUserService : IBanUserService
    {
        private readonly IBanUserRepository _banUserRepository;
        public BanUserService(IBanUserRepository banUserRepository)
        {
            _banUserRepository = banUserRepository;
        }
        public async Task<bool> BanUserAsync(int userId)
        {
            var user = await _banUserRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.IsBan = true;
            await _banUserRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> UnbanUserAsync(int userId)
        {
            var user = await _banUserRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.IsBan = false;
            await _banUserRepository.UpdateAsync(user);
            return true;
        }
    }
}
