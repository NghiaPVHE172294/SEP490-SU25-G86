using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AccountRepository
{
    public interface IAccountRepository
    {
        Account? GetByEmail(string email);
        Task<Account?> GetByIdAsync(int accountId);
        Task UpdatePasswordAsync(Account account);
    }
} 