using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

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

    }
}
