using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.ExperienceLevelRepository
{
    public interface IExperienceLevelRepository
    {
        Task<List<ExperienceLevel>> GetAllAsync();
        void Add(ExperienceLevel entity);
        Task<int> SaveChangesAsync();
    }
} 