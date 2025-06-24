using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using System.Linq;

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


        public async Task<List<Company>> GetAllCompaniesAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        public async Task<List<int>> GetEmployerIdsByCompanyIdAsync(int companyId)
        {
            var employerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "EMPLOYER");
            if (employerRole == null) return new List<int>();

            return await _context.Users
                .Where(u => u.CompanyId == companyId && u.Account.RoleId == employerRole.RoleId)
                .Select(u => u.UserId)
                .ToListAsync();
        }

        public async Task<List<int>> GetJobPostIdsByEmployerIdsAsync(List<int> employerIds)
        {
            return await _context.JobPosts
                .Where(j => employerIds.Contains(j.EmployerId ?? 0))
                .Select(j => j.JobPostId)
                .ToListAsync();
        }

        public async Task<List<Cvsubmission>> GetCVSubmissionsByJobPostIdsAsync(List<int> jobPostIds)
        {
            return await _context.Cvsubmissions
                .Where(c => jobPostIds.Contains(c.JobPostId ?? 0))
                .ToListAsync();
        }
    }

}
