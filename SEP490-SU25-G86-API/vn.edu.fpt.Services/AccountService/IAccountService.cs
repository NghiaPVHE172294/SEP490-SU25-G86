using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AccountService
{
    public interface IAccountService
    {
        Account? Authenticate(string email, string password);
        Account? GetByEmail(string email);
        // Có thể bổ sung các method khác nếu cần
    }
} 