using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CompanyRepository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public CompanyRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies
                .Include(c => c.Industry)
                .Include(c => c.CompanyFollowers)
                .FirstOrDefaultAsync(c => c.CompanyId == id);
        }

        public async Task<(List<CompanyListDTO> Companies, int TotalCount)> GetCompanyListWithJobPostCountAsync(int page, int pageSize)
        {
            var query = _context.Companies
                .Where(c => !c.IsDelete && c.Status);

            var totalCount = await query.CountAsync();

            var companies = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CompanyListDTO
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName,
                    Website = c.Website,
                    CompanySize = c.CompanySize,
                    Email = c.Email,
                    Phone = c.Phone,
                    Address = c.Address,
                    Description = c.Description,
                    LogoUrl = c.LogoUrl,
                    IndustryName = c.Industry.IndustryName,
                    FollowerCount = c.CompanyFollowers.Count(),
                    TotalJobPostEnabled = c.Users
                        .SelectMany(u => u.JobPosts)
                        .Count(jp => !jp.IsDelete && jp.Status == "OPEN")
                })
                .ToListAsync();

            return (companies, totalCount);
        }
    }
}
