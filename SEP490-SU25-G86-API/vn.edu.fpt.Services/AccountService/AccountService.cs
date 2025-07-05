using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AccountDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AccountRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AccountService;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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

        public async Task<bool> ChangePasswordAsync(ChangePasswordDTO dto)
        {
            var account = await _accountRepository.GetByIdAsync(dto.AccountId);
            if (account == null)
                throw new Exception("Không tìm thấy tài khoản.");

            string hashedCurrent = GetMd5HashStatic(dto.CurrentPassword);
            if (account.Password != hashedCurrent)
                throw new Exception("Mật khẩu hiện tại không đúng.");

            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new Exception("Mật khẩu mới không khớp.");

            if (!IsStrongPassword(dto.NewPassword))
                throw new Exception("Mật khẩu mới phải có ít nhất 1 chữ hoa, 1 chữ thường, 1 số và tối thiểu 6 ký tự.");
            
            account.Password = GetMd5HashStatic(dto.NewPassword);
            await _accountRepository.UpdatePasswordAsync(account);

            return true;
        }
        private bool IsStrongPassword(string password)
        {
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$");
            return regex.IsMatch(password);
        }
    }
}