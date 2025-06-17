using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AdminAccountDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminAccountRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminAccoutServices
{
    public class AccountListService : IAccountListService
    {
        private readonly IAccountListRepository _accountListRepository;
        public AccountListService(IAccountListRepository accountListRepository)
        {
            _accountListRepository = accountListRepository;
        }
        public async Task<IEnumerable<AccountDTOForList>> GetAllAccountsAsync()
        {
            return await _accountListRepository.GetAllAccountsAsync();
        }

        public async Task<IEnumerable<AccountDTOForList>> GetAccountsByRoleAsync(string roleName)
        {
            return await _accountListRepository.GetAccountsByRoleAsync(roleName);
        }
    }
}
