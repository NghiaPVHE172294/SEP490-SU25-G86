using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AddCompanyRepository
{
    public class InfoCompanyRepository : IInfoCompanyRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;

        public InfoCompanyRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByAccountIdAsync(int accountId)
        {
            return await _context.Companies
                .Include(c => c.Industry)
                .FirstOrDefaultAsync(c => c.CreatedByUserId == accountId && c.IsDelete == false);
        }

        public async Task<Company?> GetByIdAsync(int companyId)
        {
            return await _context.Companies
                .Include(c => c.Industry)
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.IsDelete == false);
        }

        public async Task CreateAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            _context.Companies.Update(company);
            await _context.SaveChangesAsync();
        }
        public async Task<bool> IsDuplicateCompanyAsync(CompanyCreateUpdateDTO dto)
        {
            return await _context.Companies.AnyAsync(c =>
                !c.IsDelete &&
                (c.CompanyName == dto.CompanyName ||
                 c.TaxCode == dto.TaxCode ||
                 c.Email == dto.Email ||
                 c.Phone == dto.Phone ||
                 (!string.IsNullOrEmpty(dto.Website) && c.Website == dto.Website))
            );
        }

    }
}
