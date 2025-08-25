using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPositionDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobPositionRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPositionService
{
    public class JobPositionService : IJobPositionService
    {
        private readonly IJobPositionRepository _repo;
        private readonly IMapper _mapper;
        public JobPositionService(IJobPositionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<List<JobPositionDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(jp => new JobPositionDTO { PositionId = jp.PositionId, PostitionName = jp.PostitionName }).ToList();
        }
        public async Task<int> AddAsync(AddJobPositionDTO dto)
        {
            var entity = _mapper.Map<JobPosition>(dto);
            _repo.Add(entity);
            await _repo.SaveChangesAsync();
            return entity.PositionId;
        }

        public async Task<List<JobPositionDTO>> GetByIndustryIdAsync(int industryId)
        {
            var positions = await _repo.GetByIndustryIdAsync(industryId);
            return positions.Select(jp => new JobPositionDTO 
            { 
                PositionId = jp.PositionId, 
                PostitionName = jp.PostitionName,
                IndustryId = jp.IndustryId ?? 0
            }).ToList();
        }
    }
} 