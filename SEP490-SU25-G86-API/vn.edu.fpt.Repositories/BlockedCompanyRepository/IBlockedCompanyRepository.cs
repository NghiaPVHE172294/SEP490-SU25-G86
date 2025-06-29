using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BlockedCompanyRepository
{
    public interface IBlockedCompanyRepository
    {
        Task<IEnumerable<BlockedCompany>> GetBlockedCompaniesByCandidateIdAsync(int candidateId);
        Task<BlockedCompany?> GetByIdAsync(int blockedCompanyId);
        Task<BlockedCompany> AddAsync(BlockedCompany entity);
        Task<bool> DeleteAsync(int blockedCompanyId);
    }
} 