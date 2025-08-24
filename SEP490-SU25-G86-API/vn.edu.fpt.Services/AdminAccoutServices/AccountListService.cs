using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AdminAccountDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;
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
        public Task<PagedResponse<AccountDTOForList>> GetAccountsAsync(string? roleName, string? name, int pageNumber, int pageSize, CancellationToken ct = default)
        => _accountListRepository.GetAccountsAsync(roleName, name, pageNumber, pageSize, ct);
    }
}
