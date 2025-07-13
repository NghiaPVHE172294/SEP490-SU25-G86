using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BanUserRepository
{
    public class BanUserRepository : IBanUserRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public BanUserRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
