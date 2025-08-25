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
        public async Task<bool> FollowCompanyAsync(int userId, int companyId)
        {
            // Nếu đang block thì gỡ block trước
            var blocked = await _context.BlockedCompanies
                .FirstOrDefaultAsync(x => x.CandidateId == userId && x.CompanyId == companyId);
            if (blocked != null)
            {
                _context.BlockedCompanies.Remove(blocked);
                await _context.SaveChangesAsync();
            }

            // Toggle follow
            var follow = await _context.CompanyFollowers
                .FirstOrDefaultAsync(x => x.UserId == userId && x.CompanyId == companyId);

            if (follow == null)
            {
                follow = new CompanyFollower
                {
                    UserId = userId,
                    CompanyId = companyId,
                    FlowedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.CompanyFollowers.Add(follow);
                await _context.SaveChangesAsync();
                return true; // đang follow
            }

            // Toggle trạng thái
            follow.IsActive = !(follow.IsActive ?? false);
            follow.FlowedAt = DateTime.UtcNow;
            _context.CompanyFollowers.Update(follow);
            await _context.SaveChangesAsync();

            return follow.IsActive ?? false;
        }

        public async Task<bool> BlockCompanyAsync(int userId, int companyId, string? reason)
        {
            // Nếu đang follow thì gỡ follow trước
            var follow = await _context.CompanyFollowers.FirstOrDefaultAsync(x => x.UserId == userId
                                                                               && x.CompanyId == companyId
                                                                               && x.IsActive == true);
            if (follow != null)
            {
                follow.IsActive = false;
                _context.CompanyFollowers.Update(follow);
                await _context.SaveChangesAsync();
            }

            // Toggle block
            var block = await _context.BlockedCompanies
                .FirstOrDefaultAsync(x => x.CandidateId == userId && x.CompanyId == companyId);

            if (block == null)
            {
                block = new BlockedCompany
                {
                    CandidateId = userId,
                    CompanyId = companyId,
                    Reason = reason
                };
                _context.BlockedCompanies.Add(block);
                await _context.SaveChangesAsync();
                return true; // đang block
            }
            else
            {
                _context.BlockedCompanies.Remove(block);
                await _context.SaveChangesAsync();
                return false; // đã unblock
            }
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
