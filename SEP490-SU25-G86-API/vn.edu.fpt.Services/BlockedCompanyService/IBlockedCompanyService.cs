using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.BlockedCompanyService
{
    public interface IBlockedCompanyService
    {
        Task<IEnumerable<BlockedCompanyDTO>> GetBlockedCompaniesByCandidateIdAsync(int candidateId);
        Task<BlockedCompany?> GetByIdAsync(int blockedCompanyId);
        Task<BlockedCompany> AddAsync(BlockedCompany entity);
        Task<bool> DeleteAsync(int blockedCompanyId);
    }
} 