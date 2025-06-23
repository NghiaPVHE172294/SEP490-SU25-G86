using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminDashboardDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminDashboardServices
{
    public interface IAdminDashboardService
    {
        List<JobPostMonthlyStatisticDTO> GetMonthlyJobPostStatistics();

        Task<List<CompanyToGetDTO>> GetAllCompaniesAsync();
        Task<List<CVSubmissionStatisticDTO>> GetCVSubmissionStatsAsync(int companyId, string mode);
    }
}
