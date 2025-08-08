using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.HandbookCategoryDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.HandbookCategoryRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.HandbookCategoryService
{
    public class HandbookCategoryService : IHandbookCategoryService
    {
        private readonly IHandbookCategoryRepository _repository;

        public HandbookCategoryService(IHandbookCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<HandbookCategoryDetailDTO>> GetAllAsync()
        {
            var data = await _repository.GetAllAsync();
            return data.Select(MapToDetailDto).ToList();
        }

        public async Task<HandbookCategoryDetailDTO?> GetByIdAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            return category == null ? null : MapToDetailDto(category);
        }

        public async Task<bool> CreateAsync(HandbookCategoryCreateDTO dto)
        {
            if (await _repository.ExistsByNameAsync(dto.CategoryName))
                throw new Exception("Tên danh mục đã tồn tại");

            var category = new HandbookCategory
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(category);
            return true;
        }

        public async Task<bool> UpdateAsync(int id, HandbookCategoryUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            if (await _repository.ExistsByNameAsync(dto.CategoryName, id))
                throw new Exception("Tên danh mục đã tồn tại");

            existing.CategoryName = dto.CategoryName;
            existing.Description = dto.Description;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            if (await _repository.HasCareerHandbooksAsync(id))
                throw new Exception("Không thể xóa danh mục đang được sử dụng");

            await _repository.DeleteAsync(existing);
            return true;
        }

        private HandbookCategoryDetailDTO MapToDetailDto(HandbookCategory c) => new()
        {
            CategoryId = c.CategoryId,
            CategoryName = c.CategoryName,
            Description = c.Description,
            CreatedAt = c.CreatedAt
        };
    }
}
