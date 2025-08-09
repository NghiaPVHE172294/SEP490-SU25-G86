using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.HandbookCategoryDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.HandbookCategoryService
{
    public interface IHandbookCategoryService
    {
        Task<List<HandbookCategoryDetailDTO>> GetAllAsync();
        Task<HandbookCategoryDetailDTO?> GetByIdAsync(int id);
        Task<bool> CreateAsync(HandbookCategoryCreateDTO dto);
        Task<bool> UpdateAsync(int id, HandbookCategoryUpdateDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
