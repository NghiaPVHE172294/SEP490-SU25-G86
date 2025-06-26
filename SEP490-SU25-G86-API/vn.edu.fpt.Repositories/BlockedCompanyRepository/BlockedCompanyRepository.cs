using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BlockedCompanyRepository
{
    public class BlockedCompanyRepository : IBlockedCompanyRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public BlockedCompanyRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlockedCompany>> GetBlockedCompaniesByCandidateIdAsync(int candidateId)
        {
            return await _context.BlockedCompanies
                .Include(bc => bc.Company)
                .Where(bc => bc.CandidateId == candidateId)
                .ToListAsync();
        }

        public async Task<BlockedCompany?> GetByIdAsync(int blockedCompanyId)
        {
            return await _context.BlockedCompanies
                .Include(bc => bc.Company)
                .FirstOrDefaultAsync(bc => bc.BlockedCompaniesId == blockedCompanyId);
        }

        public async Task<BlockedCompany> AddAsync(BlockedCompany entity)
        {
            _context.BlockedCompanies.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> DeleteAsync(int blockedCompanyId)
        {
            var entity = await _context.BlockedCompanies.FindAsync(blockedCompanyId);
            if (entity == null) return false;
            _context.BlockedCompanies.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 