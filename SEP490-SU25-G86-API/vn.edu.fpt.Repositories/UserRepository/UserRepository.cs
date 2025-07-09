using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public UserRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByAccountIdAsync(int accountId)
        {
            return await _context.Users
                .Include(u => u.Account)
                .FirstOrDefaultAsync(u => u.AccountId == accountId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
