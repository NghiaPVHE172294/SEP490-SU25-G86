using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobLevelDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.JobLevelRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.JobLevelService
{
    public class JobLevelService : IJobLevelService
    {
        private readonly IJobLevelRepository _repo;
        private readonly IMapper _mapper;
        public JobLevelService(IJobLevelRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<List<JobLevelDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(jl => new JobLevelDTO { JobLevelId = jl.JobLevelId, JobLevelName = jl.JobLevelName }).ToList();
        }
        public async Task<int> AddAsync(AddJobLevelDTO dto)
        {
            var entity = _mapper.Map<JobLevel>(dto);
            _repo.Add(entity);
            await _repo.SaveChangesAsync();
            return entity.JobLevelId;
        }
    }
} 