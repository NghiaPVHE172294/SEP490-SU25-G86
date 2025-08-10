using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CareerHandbookService
{
    public interface ICareerHandbookService
    {
        // Admin
        Task<List<CareerHandbookDetailDTO>> GetAllForAdminAsync();
        Task<CareerHandbookDetailDTO?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CareerHandbookCreateDTO dto);
        Task<bool> UpdateAsync(int id, CareerHandbookUpdateDTO dto);
        Task<bool> SoftDeleteAsync(int id); // thêm hàm delete mềm

        // User
        Task<List<CareerHandbookDetailDTO>> GetAllPublishedAsync();
        Task<CareerHandbookDetailDTO?> GetBySlugAsync(string slug);

        //Task<string> UploadThumbnailAsync(IFormFile file);
    }
}
