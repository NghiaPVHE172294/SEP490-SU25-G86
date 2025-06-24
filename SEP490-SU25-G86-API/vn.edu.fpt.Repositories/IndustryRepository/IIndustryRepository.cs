using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.IndustryRepository
{
    public interface IIndustryRepository
    {
        Task<IEnumerable<Industry>> GetAllAsync();
    }
}
