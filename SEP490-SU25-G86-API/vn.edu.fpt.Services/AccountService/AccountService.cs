using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AccountRepository;
using System.Security.Cryptography;
using System.Text;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AccountService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public Account? Authenticate(string email, string password)
        {
            var account = _accountRepository.GetByEmail(email);
            if (account == null) return null;
            // Hash password nhập vào bằng MD5
            string hashedInput = GetMd5Hash(password);
            if (account.Password != hashedInput) return null;
            return account;
        }

        private string GetMd5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                // Chuyển sang chuỗi hex
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public static string GetMd5HashStatic(string input)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);
                // Chuyển sang chuỗi hex
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public Account? GetByEmail(string email)
        {
            return _accountRepository.GetByEmail(email);
        }
    }
}