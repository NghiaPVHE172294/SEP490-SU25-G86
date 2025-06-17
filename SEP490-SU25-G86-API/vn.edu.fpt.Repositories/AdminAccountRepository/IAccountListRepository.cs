
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AdminAccountDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminAccountRepository
{
    public interface IAccountListRepository
    {
        Task<IEnumerable<AccountDTOForList>> GetAllAccountsAsync();
        Task<IEnumerable<AccountDTOForList>> GetAccountsByRoleAsync(string roleName);
    }
}
