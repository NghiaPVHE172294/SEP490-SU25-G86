using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService
{
    public interface ICvService
    {
        Task<List<CvDTO>> GetAllByUserAsync(int userId);
        Task<CvDTO?> GetByIdAsync(int cvId);
        Task<int> AddAsync(int userId, string roleName, AddCvDTO dto, string fileUrl);
        Task DeleteAsync(int userId, int cvId);
        Task UpdateCvNameAsync(int cvId, string newName);
        Task<string> UploadFileToFirebaseStorage(IFormFile file, int candidateId);
        Task<string> UploadFileToFirebaseStorage(IFormFile file); // overload cũ để không lỗi các controller khác
    }
} 