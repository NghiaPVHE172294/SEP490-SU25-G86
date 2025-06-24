using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ProvinceRepository
{
    public interface IProvinceRepository
    {
        Task<List<Province>> GetAllAsync();
        void Add(Province entity);
        Task<int> SaveChangesAsync();
    }
}
