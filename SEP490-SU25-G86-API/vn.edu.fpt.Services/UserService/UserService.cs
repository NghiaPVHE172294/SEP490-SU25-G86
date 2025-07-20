using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CVRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserRepository;
using System.Globalization;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;

        public UserService(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<UserProfileDTO?> GetUserProfileAsync(int accountId)
        {
            var user = await _userRepo.GetUserByAccountIdAsync(accountId);
            if (user == null) return null;

            return new UserProfileDTO
            {
                Avatar = user.Avatar,
                FullName = user.FullName,
                Address = user.Address,
                Email = user.Account.Email,
                Phone = user.Phone,
                Dob = user.Dob?.ToString("dd/MM/yyyy"),
                LinkedIn = user.LinkedIn,
                Facebook = user.Facebook,
                AboutMe = user.AboutMe
            };
        }

        public async Task<bool> UpdateUserProfileAsync(int accountId, UpdateUserProfileDTO dto)
        {
            var user = await _userRepo.GetUserByAccountIdAsync(accountId);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.FullName = dto.FullName;

            if (dto.Address != null)
                user.Address = dto.Address;

            if (dto.Phone != null)
                user.Phone = dto.Phone;

            if (!string.IsNullOrWhiteSpace(dto.Dob))
            {
                if (DateTime.TryParseExact(dto.Dob, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDob))
                {
                    user.Dob = parsedDob;
                }
            }

            if (dto.LinkedIn != null)
                user.LinkedIn = dto.LinkedIn;

            if (dto.Facebook != null)
                user.Facebook = dto.Facebook;

            if (dto.AboutMe != null)
                user.AboutMe = dto.AboutMe;

            if (dto.Avatar != null)
                user.Avatar = dto.Avatar;

            await _userRepo.UpdateUserAsync(user);
            return true;
        }
        public async Task<bool> FollowCompanyAsync(int userId, int companyId)
        {
            return await _userRepo.FollowCompanyAsync(userId, companyId);
        }

        public async Task<bool> BlockCompanyAsync(int userId, int companyId, string? reason)
        {
            return await _userRepo.BlockCompanyAsync(userId, companyId, reason);
        }
    }
}
