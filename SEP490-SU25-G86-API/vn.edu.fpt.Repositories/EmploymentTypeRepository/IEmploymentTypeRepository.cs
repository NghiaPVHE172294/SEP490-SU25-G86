using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.EmploymentTypeRepository
{
    public interface IEmploymentTypeRepository
    {
        Task<List<EmploymentType>> GetAllAsync();
        void Add(EmploymentType entity);
        Task<int> SaveChangesAsync();
    }
} 