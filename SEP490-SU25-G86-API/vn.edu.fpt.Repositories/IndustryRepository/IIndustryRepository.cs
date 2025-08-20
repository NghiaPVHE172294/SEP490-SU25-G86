using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.IndustryDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.IndustryRepository
{
    public interface IIndustryRepository
    {
        Task<IEnumerable<Industry>> GetAllAsync();
        void Add(Industry entity);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<(Industry Industry, int JobPostCount)>> GetIndustriesWithJobPostCount(int page, int pageSize);
    }
}
