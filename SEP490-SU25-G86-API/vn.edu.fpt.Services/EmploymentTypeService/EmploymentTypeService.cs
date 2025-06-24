using SEP490_SU25_G86_API.vn.edu.fpt.DTO.EmploymentTypeDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.EmploymentTypeRepository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.EmploymentTypeService
{
    public class EmploymentTypeService : IEmploymentTypeService
    {
        private readonly IEmploymentTypeRepository _repo;
        private readonly IMapper _mapper;
        public EmploymentTypeService(IEmploymentTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        public async Task<List<EmploymentTypeDTO>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(e => new EmploymentTypeDTO { EmploymentTypeId = e.EmploymentTypeId, EmploymentTypeName = e.EmploymentTypeName }).ToList();
        }
        public async Task<int> AddAsync(AddEmploymentTypeDTO dto)
        {
            var entity = _mapper.Map<EmploymentType>(dto);
            _repo.Add(entity);
            await _repo.SaveChangesAsync();
            return entity.EmploymentTypeId;
        }
    }
} 