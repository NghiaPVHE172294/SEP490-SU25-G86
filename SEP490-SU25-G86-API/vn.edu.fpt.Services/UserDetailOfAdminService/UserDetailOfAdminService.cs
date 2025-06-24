using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserDetailOfAdminRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.UserDetailOfAdminService
{
    public class UserDetailOfAdminService : IUserDetailOfAdminService
    {
        private readonly IUserDetailOfAdminRepository _repository;

        public UserDetailOfAdminService(IUserDetailOfAdminRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDetailOfAdminDTO> GetUserDetailByAccountIdAsync(int accountId)
        {
            return await _repository.GetUserDetailByAccountIdAsync(accountId);
        }
    }
}
