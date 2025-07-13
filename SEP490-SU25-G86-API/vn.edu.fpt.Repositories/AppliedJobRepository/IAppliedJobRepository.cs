using SEP490_SU25_G86_API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AppliedJobRepository
{
    public interface IAppliedJobRepository
    {
        Task<IEnumerable<Cvsubmission>> GetByUserIdAsync(int userId);
        Task<bool> HasUserAppliedToJobAsync(int userId, int jobPostId);
    }
} 