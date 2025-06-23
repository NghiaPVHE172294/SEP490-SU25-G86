using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminDashboardRepository
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly SEP490_G86_CvMatchContext _context;
        public AdminDashboardRepository(SEP490_G86_CvMatchContext context)
        {
            _context = context;
        }
        public IEnumerable<JobPost> GetAll()
        {
            return _context.JobPosts.ToList();
        }
    }
}
