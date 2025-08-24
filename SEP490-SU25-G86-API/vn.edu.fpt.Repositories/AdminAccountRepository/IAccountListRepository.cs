
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AdminAccountDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminAccountDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminAccountRepository
{
    public interface IAccountListRepository
    {
        Task<PagedResponse<AccountDTOForList>> GetAccountsAsync(
        string? roleName, string? name, int pageNumber, int pageSize, CancellationToken ct = default);
    }
}
