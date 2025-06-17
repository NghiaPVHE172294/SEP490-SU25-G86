using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyFollowingRepositories
{
    public class CompanyFollowingRepository : ICompanyFollowingRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public CompanyFollowingRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CompanyFollowingDTO>> GetFollowedCompaniesByUserIdAsync(int userId)
        {
            return await _context.CompanyFollowers
                .Where(cf => cf.UserId == userId && cf.IsActive == true)
                .Include(cf => cf.Company)
                .ThenInclude(c => c.Industry)
                .Select(cf => new CompanyFollowingDTO
                {
                    CompanyId = cf.CompanyId,
                    CompanyName = cf.Company.CompanyName,
                    LogoUrl = cf.Company.LogoUrl,
                    Website = cf.Company.Website,
                    IndustryName = cf.Company.Industry.IndustryName,
                    Description = cf.Company.Description,
                    FlowedAt = cf.FlowedAt
                })
                .ToListAsync();
        }
    }
}
