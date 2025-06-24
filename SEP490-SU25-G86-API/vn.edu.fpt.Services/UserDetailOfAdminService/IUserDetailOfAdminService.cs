using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.UserDetailOfAdminService
{
    public interface IUserDetailOfAdminService
    {
        Task<UserDetailOfAdminDTO> GetUserDetailByAccountIdAsync(int accountId);
    }
}
