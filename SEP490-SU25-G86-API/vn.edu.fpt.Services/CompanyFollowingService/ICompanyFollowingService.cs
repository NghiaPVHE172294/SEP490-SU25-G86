using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyFollowingService
{
    public interface ICompanyFollowingService
    {
        Task<IEnumerable<CompanyFollowingDTO>> GetFollowedCompaniesAsync(int userId);
    }
}
