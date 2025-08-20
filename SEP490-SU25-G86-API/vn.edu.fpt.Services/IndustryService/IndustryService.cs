using AutoMapper;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.IndustryDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.IndustryDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.IndustryRepository;

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
        public async Task<IEnumerable<IndustryWithJobCountDTO>> GetIndustriesWithJobCount(int page, int pageSize)
        {
            var data = await _industryRepo.GetIndustriesWithJobPostCount(page, pageSize);

            return data.Select(x => new IndustryWithJobCountDTO
            {
                IndustryId = x.Industry.IndustryId,
                IndustryName = x.Industry.IndustryName,
                JobPostCount = x.JobPostCount
            }).ToList();
        }

    }
}
