using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.BlockedCompanyRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.BlockedCompanyService
{
    public class BlockedCompanyService : IBlockedCompanyService
    {
        private readonly IBlockedCompanyRepository _repo;
        public BlockedCompanyService(IBlockedCompanyRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<BlockedCompanyDTO>> GetBlockedCompaniesByCandidateIdAsync(int candidateId)
        {
            var list = await _repo.GetBlockedCompaniesByCandidateIdAsync(candidateId);
            return list.Select(bc => new BlockedCompanyDTO
            {
                BlockedCompaniesId = bc.BlockedCompaniesId,
                CompanyId = bc.CompanyId,
                CompanyName = bc.Company?.CompanyName ?? string.Empty,
                LogoUrl = bc.Company?.LogoUrl,
                Reason = bc.Reason
            });
        }

        public Task<BlockedCompany?> GetByIdAsync(int blockedCompanyId)
            => _repo.GetByIdAsync(blockedCompanyId);

        public Task<BlockedCompany> AddAsync(BlockedCompany entity)
            => _repo.AddAsync(entity);

        public Task<bool> DeleteAsync(int blockedCompanyId)
            => _repo.DeleteAsync(blockedCompanyId);
    }
} 