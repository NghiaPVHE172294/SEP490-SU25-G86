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
        public async Task<bool> FollowCompanyAsync(int accountId, int companyId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
            {
                throw new Exception($"No user found for AccountId: {accountId}");
            }
            var userId = user.UserId;

            var follow = await _context.CompanyFollowers
                .FirstOrDefaultAsync(x => x.UserId == userId && x.CompanyId == companyId);

            if (follow == null)
            {
                // Chưa từng follow → tạo mới
                follow = new CompanyFollower
                {
                    UserId = userId,
                    CompanyId = companyId,
                    FlowedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.CompanyFollowers.Add(follow);
                await _context.SaveChangesAsync();
                return true;
            }

            if (!(bool)follow.IsActive)
            {
                follow.IsActive = true;
                follow.FlowedAt = DateTime.UtcNow;
                _context.CompanyFollowers.Update(follow);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                // Đang follow → hủy theo dõi
                follow.IsActive = false;
                _context.CompanyFollowers.Update(follow);
                await _context.SaveChangesAsync();
                return false;
            }
        }

        public async Task<bool> BlockCompanyAsync(int accountId, int companyId, string? reason)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
            {
                throw new Exception($"No user found for AccountId: {accountId}");
            }
            var userId = user.UserId;

            var exists = await _context.BlockedCompanies
                .AnyAsync(x => x.CandidateId == userId && x.CompanyId == companyId);

            if (!exists)
            {
                var block = new BlockedCompany
                {
                    CandidateId = userId,
                    CompanyId = companyId,
                    Reason = reason
                };
                _context.BlockedCompanies.Add(block);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> IsCompanyFollowedAsync(int accountId, int companyId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
            {
                return false;
            }
            var userId = user.UserId;

            return await _context.CompanyFollowers
                .AnyAsync(cf => cf.UserId == userId && cf.CompanyId == companyId && cf.IsActive == true);
        }

        public async Task<bool> IsCompanyBlockedAsync(int accountId, int companyId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
            {
                return false;
            }
            var userId = user.UserId;

            return await _context.BlockedCompanies
                .AnyAsync(bc => bc.CandidateId == userId && bc.CompanyId == companyId);
        }
    }
}
