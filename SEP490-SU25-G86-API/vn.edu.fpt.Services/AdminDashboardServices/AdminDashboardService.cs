using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminDashboardDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminDashboardRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminDashboardServices
{
    public class AdminDashboardService : IAdminDashboardService
    {
        readonly IAdminDashboardRepository _adminDashboardRepository;
        public AdminDashboardService(IAdminDashboardRepository adminDashboardRepository)
        {
            _adminDashboardRepository = adminDashboardRepository;
        }
        public List<JobPostMonthlyStatisticDTO> GetMonthlyJobPostStatistics()
        {
            var posts = _adminDashboardRepository.GetAll();

            var stats = posts
                .GroupBy(p => new {
                    Year = p.CreatedDate.GetValueOrDefault().Year,
                    Month = p.CreatedDate.GetValueOrDefault().Month
                })
                .Select(g => new JobPostMonthlyStatisticDTO
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(s => s.Year).ThenBy(s => s.Month)
                .ToList();

            return stats;
        }
    }
}
