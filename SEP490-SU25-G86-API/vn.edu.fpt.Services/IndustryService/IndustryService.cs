using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.IndustryRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.IndustryService
{
    public class IndustryService : IIndustryService
    {
        private readonly IIndustryRepository _industryRepo;

        public IndustryService(IIndustryRepository industryRepo)
        {
            _industryRepo = industryRepo;
        }

        public async Task<IEnumerable<Industry>> GetAllIndustriesAsync()
        {
            return await _industryRepo.GetAllAsync();
        }
    }
}
