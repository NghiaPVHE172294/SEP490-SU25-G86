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

        public async Task<(List<Company> Companies, int TotalCount)> GetPagedCompaniesAsync(int page, int pageSize)
        {
            var query = _context.Companies
        .Include(c => c.Industry)
        .Include(c => c.CompanyFollowers)
        .Include(c => c.Users)
            .ThenInclude(u => u.JobPosts)
        .Where(c => c.IsDelete == false && c.Status == false);

            var totalCount = await query.CountAsync();

            var companies = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (companies, totalCount);
        }
        public async Task<List<Company>> GetAllCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<Company?> GetLatestCompanyAsync()
        {
            // Use CompanyId as fallback since CreatedDate does not exist
            return await _context.Companies
                .Where(c => !c.IsDelete && c.Status == false)
                .OrderByDescending(c => c.CompanyId)
                .FirstOrDefaultAsync();
        }
    }
}
