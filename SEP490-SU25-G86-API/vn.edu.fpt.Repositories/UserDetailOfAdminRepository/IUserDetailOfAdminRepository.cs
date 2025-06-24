using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserDetailOfAdminRepository
{
    public interface IUserDetailOfAdminRepository
    {
        Task<UserDetailOfAdminDTO> GetUserDetailByAccountIdAsync(int accountId);
    }
}
