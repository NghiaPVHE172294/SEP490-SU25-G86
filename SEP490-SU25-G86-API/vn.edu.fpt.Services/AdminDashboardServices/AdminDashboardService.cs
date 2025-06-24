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


        public async Task<List<CompanyToGetDTO>> GetAllCompaniesAsync()
        {
            var companies = await _adminDashboardRepository.GetAllCompaniesAsync();
            return companies.Select(c => new CompanyToGetDTO
            {
                CompanyId = c.CompanyId,
                CompanyName = c.CompanyName
            }).ToList();
        }

        public async Task<List<CVSubmissionStatisticDTO>> GetCVSubmissionStatsAsync(int companyId, string mode)
        {
            var employerIds = await _adminDashboardRepository.GetEmployerIdsByCompanyIdAsync(companyId);
            if (!employerIds.Any()) return new List<CVSubmissionStatisticDTO>();

            var jobPostIds = await _adminDashboardRepository.GetJobPostIdsByEmployerIdsAsync(employerIds);
            if (!jobPostIds.Any()) return new List<CVSubmissionStatisticDTO>();

            var submissions = await _adminDashboardRepository.GetCVSubmissionsByJobPostIdsAsync(jobPostIds);

            // Lọc bỏ các bản ghi có SubmissionDate bị null
            var validSubmissions = submissions.Where(s => s.SubmissionDate.HasValue).ToList();


            if (mode == "year")
            {
                return validSubmissions
                    .GroupBy(s => s.SubmissionDate.Value.Year)
                    .Select(g => new CVSubmissionStatisticDTO
                    {
                        Year = g.Key,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Year)
                    .ToList();
            }
            else
            {
                return validSubmissions
                    .GroupBy(s => new { Year = s.SubmissionDate.Value.Year, Month = s.SubmissionDate.Value.Month })
                    .Select(g => new CVSubmissionStatisticDTO
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        Count = g.Count()
                    })
                    .OrderBy(g => g.Year).ThenBy(g => g.Month)
                    .ToList();
            }
        }
    }
}
