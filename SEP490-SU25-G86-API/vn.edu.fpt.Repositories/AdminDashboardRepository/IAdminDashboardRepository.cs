using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminDashboardRepository
{
    public interface IAdminDashboardRepository
    {
        IEnumerable<JobPost> GetAll();


        Task<List<Company>> GetAllCompaniesAsync();
        Task<List<int>> GetEmployerIdsByCompanyIdAsync(int companyId);
        Task<List<int>> GetJobPostIdsByEmployerIdsAsync(List<int> employerIds);
        Task<List<Cvsubmission>> GetCVSubmissionsByJobPostIdsAsync(List<int> jobPostIds);
    }
}
