using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.SalaryRangeRepository
{
    public interface ISalaryRangeRepository
    {
        Task<List<SalaryRange>> GetAllAsync();
        Task AddAsync(SalaryRange entity);
    }
} 