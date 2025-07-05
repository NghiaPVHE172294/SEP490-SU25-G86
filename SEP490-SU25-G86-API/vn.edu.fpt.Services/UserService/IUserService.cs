using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.UserService
{
    public interface IUserService
    {
        Task<UserProfileDTO?> GetUserProfileAsync(int accountId);
        Task<bool> UpdateUserProfileAsync(int accountId, UserProfileDTO dto);
    }
}
