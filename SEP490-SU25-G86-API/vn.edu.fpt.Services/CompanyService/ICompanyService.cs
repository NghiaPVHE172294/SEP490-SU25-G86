using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CompanyDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyService
{
    public interface ICompanyService
    {
        Task<CompanyDTO?> GetCompanyDtoByIdAsync(int id);
        Task<CompanyDTO?> GetLatestCompanyDtoAsync();
        Task<(List<CompanyListDTO> Companies, int TotalCount)> GetPagedCompanyListWithJobPostCountAsync(int page, int pageSize);
        Task<List<CompanyLogoDTO>> GetAllCompanyLogos();
    }
}
