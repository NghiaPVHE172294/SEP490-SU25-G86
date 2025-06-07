using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services
{
    public class AccountService
    {
        private readonly AccountRepository _accountRepository;
        public AccountService(AccountRepository accountRepository)
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
            using (var md5 = System.Security.Cryptography.MD5.Create())
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