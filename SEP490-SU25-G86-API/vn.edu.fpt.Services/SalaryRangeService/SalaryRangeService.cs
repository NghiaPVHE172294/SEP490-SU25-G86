using SEP490_SU25_G86_API.vn.edu.fpt.DTO.SalaryRangeDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SalaryRangeRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.SalaryRangeService
{
    public class SalaryRangeService : ISalaryRangeService
    {
        private readonly ISalaryRangeRepository _repo;
        public SalaryRangeService(ISalaryRangeRepository repo)
        {
            _repo = repo;
        }
        public async Task<List<SalaryRangeDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(s => new SalaryRangeDTO
            {
                SalaryRangeId = s.SalaryRangeId,
                MinSalary = s.MinSalary,
                MaxSalary = s.MaxSalary,
                Currency = s.Currency
            }).ToList();
        }
        public async Task<int> AddAsync(AddSalaryRangeDTO dto)
        {
            var entity = new Models.SalaryRange
            {
                MinSalary = dto.MinSalary,
                MaxSalary = dto.MaxSalary,
                Currency = dto.Currency
            };
            var repo = _repo as Repositories.SalaryRangeRepository.SalaryRangeRepository;
            if (repo != null)
            {
                await repo.AddAsync(entity);
                return entity.SalaryRangeId;
            }
            throw new System.Exception("Repository không hỗ trợ thêm mới");
        }
    }
} 