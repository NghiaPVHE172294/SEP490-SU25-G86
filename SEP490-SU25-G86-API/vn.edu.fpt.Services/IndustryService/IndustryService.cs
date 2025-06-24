using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.IndustryRepository;
using AutoMapper;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.IndustryDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.IndustryService
{
    public class IndustryService : IIndustryService
    {
        private readonly IIndustryRepository _industryRepo;
        private readonly IMapper _mapper;

        public IndustryService(IIndustryRepository industryRepo, IMapper mapper)
        {
            _industryRepo = industryRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Industry>> GetAllIndustriesAsync()
        {
            return await _industryRepo.GetAllAsync();
        }

        public async Task<int> AddAsync(AddIndustryDTO dto)
        {
            var entity = _mapper.Map<Industry>(dto);
            _industryRepo.Add(entity);
            await _industryRepo.SaveChangesAsync();
            return entity.IndustryId;
        }
    }
}
