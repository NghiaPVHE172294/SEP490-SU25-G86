using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO;
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

            user.Avatar = dto.Avatar;
            user.FullName = dto.FullName;
            user.Address = dto.Address;
            user.Phone = dto.Phone;
            if (DateTime.TryParseExact(dto.Dob, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dob))
            {
                user.Dob = dob.Date;
            }
            user.LinkedIn = dto.LinkedIn;
            user.Facebook = dto.Facebook;
            user.AboutMe = dto.AboutMe;

            await _userRepo.UpdateUserAsync(user);
            return true;
        }
    }
}
