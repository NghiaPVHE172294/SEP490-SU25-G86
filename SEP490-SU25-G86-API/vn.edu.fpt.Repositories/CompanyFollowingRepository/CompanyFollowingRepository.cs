using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyFollowingRepositories
{
    public class CompanyFollowingRepository : ICompanyFollowingRepository
    {
        // Đếm tổng số công ty đã follow
        public async Task<int> CountFollowedCompaniesAsync(int userId)
        {
            return await _context.CompanyFollowers.CountAsync(cf => cf.UserId == userId && cf.IsActive == true);
        }
    
        private readonly SEP490_G86_CvMatchContext _context;

        public CompanyFollowingRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompanyFollowingDTO>> GetFollowedCompaniesByUserIdAsync(int userId, int page, int pageSize)
        {
            return await _context.CompanyFollowers
                .Where(cf => cf.UserId == userId && cf.IsActive == true)
                .Include(cf => cf.Company)
                .ThenInclude(c => c.Industry)
                .OrderByDescending(cf => cf.FlowedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(cf => new CompanyFollowingDTO
                {
                    CompanyId = cf.CompanyId,
                    CompanyName = cf.Company.CompanyName,
                    LogoUrl = cf.Company.LogoUrl,
                    Website = cf.Company.Website,
                    IndustryName = cf.Company.Industry.IndustryName,
                    Description = cf.Company.Description,
                    FlowedAt = cf.FlowedAt,
                    Location = cf.Company.Address,
                    ActiveJobsCount = _context.JobPosts.Count(jp => jp.Employer.CompanyId == cf.CompanyId && jp.Status == "OPEN" && (!jp.EndDate.HasValue || jp.EndDate.Value.Date >= DateTime.UtcNow.Date)),
                    FollowId = cf.FollowId
                })
                .ToListAsync();
        }

        public async Task<int> CountSuggestedCompaniesAsync(int userId)
        {
            var blockedCompanyIds = await _context.BlockedCompanies
                .Where(bc => bc.CandidateId == userId)
                .Select(bc => bc.CompanyId)
                .ToListAsync();
            return await _context.Companies.CountAsync(c => !blockedCompanyIds.Contains(c.CompanyId));
        }

        public async Task<IEnumerable<CompanyFollowingDTO>> GetSuggestedCompaniesAsync(int userId, int page, int pageSize)
        {
            
            // Lấy danh sách công ty đã bị block
            var blockedCompanyIds = await _context.BlockedCompanies
                .Where(bc => bc.CandidateId == userId)
                .Select(bc => bc.CompanyId)
                .ToListAsync();

            return await _context.Companies
                .Where(c => !blockedCompanyIds.Contains(c.CompanyId))
                .Include(c => c.Industry)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CompanyFollowingDTO
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    LogoUrl = c.LogoUrl,
                    Website = c.Website,
                    IndustryName = c.Industry.IndustryName,
                    Description = c.Description,
                    FlowedAt = DateTime.Now // Không có ngày follow vì là suggest, có thể để mặc định hoặc null
                })
                .ToListAsync();
        }

        public async Task<bool> UnfollowCompanyAsync(int followId)
        {
            var follow = await _context.CompanyFollowers.FirstOrDefaultAsync(cf => cf.FollowId == followId);

            if (follow == null || follow.IsActive == false)
                return false;

            follow.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
