using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.CareerHandbookRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.CareerHandbookService
{
    

    public class CareerHandbookService : ICareerHandbookService
    {
        private readonly ICareerHandbookRepository _repository;

        public CareerHandbookService(ICareerHandbookRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CareerHandbookDetailDTO>> GetAllForAdminAsync()
        {
            var data = await _repository.GetAllAsync(false);
            return data.Select(MapToDetailDto).ToList();
        }

        public async Task<List<CareerHandbookDetailDTO>> GetAllPublishedAsync()
        {
            var data = await _repository.GetAllPublishedAsync();
            return data.Select(MapToDetailDto).ToList();
        }

        public async Task<CareerHandbookDetailDTO?> GetByIdAsync(int id)
        {
            var handbook = await _repository.GetByIdAsync(id);
            return handbook == null ? null : MapToDetailDto(handbook);
        }

        public async Task<CareerHandbookDetailDTO?> GetBySlugAsync(string slug)
        {
            var handbook = await _repository.GetBySlugAsync(slug);
            return handbook == null ? null : MapToDetailDto(handbook);
        }

        public async Task<bool> CreateAsync(CareerHandbookCreateDTO dto)
        {
            if (await _repository.ExistsBySlugAsync(dto.Slug))
                throw new Exception("Slug đã tồn tại");

            var handbook = new CareerHandbook
            {
                Title = dto.Title,
                Slug = dto.Slug,
                Content = dto.Content,
                ThumbnailUrl = dto.ThumbnailUrl,
                Tags = dto.Tags,
                CategoryId = dto.CategoryId,
                IsPublished = dto.IsPublished,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _repository.AddAsync(handbook);
            return true;
        }

        public async Task<bool> UpdateAsync(int id, CareerHandbookUpdateDTO dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            if (await _repository.ExistsBySlugAsync(dto.Slug, id))
                throw new Exception("Slug đã tồn tại");

            existing.Title = dto.Title;
            existing.Slug = dto.Slug;
            existing.Content = dto.Content;
            existing.ThumbnailUrl = dto.ThumbnailUrl;
            existing.Tags = dto.Tags;
            existing.CategoryId = dto.CategoryId;
            existing.IsPublished = dto.IsPublished;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            await _repository.SoftDeleteAsync(id);
            return true;
        }


        private CareerHandbookDetailDTO MapToDetailDto(CareerHandbook h) => new()
        {
            HandbookId = h.HandbookId,
            Title = h.Title,
            Slug = h.Slug,
            Content = h.Content,
            ThumbnailUrl = h.ThumbnailUrl,
            Tags = h.Tags,
            CategoryId = h.CategoryId,
            CategoryName = h.Category.CategoryName,
            IsPublished = h.IsPublished,
            CreatedAt = h.CreatedAt,
            UpdatedAt = h.UpdatedAt
        };
    }
}
