using SEP490_SU25_G86_API.vn.edu.fpt.DTO.ExperienceLevelDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ExperienceLevelRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.ExperienceLevelService
{
    public class ExperienceLevelService : IExperienceLevelService
    {
        private readonly IExperienceLevelRepository _repo;
        private readonly IMapper _mapper;
        public ExperienceLevelService(IExperienceLevelRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<List<ExperienceLevelDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => new ExperienceLevelDTO { ExperienceLevelId = e.ExperienceLevelId, ExperienceLevelName = e.ExperienceLevelName }).ToList();
        }
        public async Task<int> AddAsync(AddExperienceLevelDTO dto)
        {
            var entity = _mapper.Map<ExperienceLevel>(dto);
            _repo.Add(entity);
            await _repo.SaveChangesAsync();
            return entity.ExperienceLevelId;
        }
    }
} 