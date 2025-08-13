using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
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

            if (dto.ThumbnailFile != null)
            {
                dto.ThumbnailUrl = await UploadThumbnailToFirebase(dto.ThumbnailFile);
            }

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

            if (dto.ThumbnailFile != null)
            {
                dto.ThumbnailUrl = await UploadThumbnailToFirebase(dto.ThumbnailFile);
            }

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

        private async Task<string> UploadThumbnailToFirebase(IFormFile file)
        {
            string firebaseCredentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS")
                ?? "D:\\FPTU\\SEP490_SUMMER25_G86\\sep490-su25-g86-cvmatcher-25bbfc6aba06.json";

            if (!File.Exists(firebaseCredentialsPath))
                throw new FileNotFoundException($"Không tìm thấy file Firebase credentials tại {firebaseCredentialsPath}");

            string bucketName = Environment.GetEnvironmentVariable("FIREBASE_BUCKET")
                ?? "sep490-su25-g86-cvmatcher.firebasestorage.app";

            string folderName = "Image_storage/CareerHandbookThumbnail";

            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                FirebaseAdmin.FirebaseApp.Create(new FirebaseAdmin.AppOptions()
                {
                    Credential = GoogleCredential.FromFile(firebaseCredentialsPath),
                });
            }

            var credential = GoogleCredential.FromFile(firebaseCredentialsPath);
            var storage = StorageClient.Create(credential);

            using var stream = file.OpenReadStream();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string objectName = $"{folderName}/{timestamp}_{file.FileName}";

            var contentType = string.IsNullOrEmpty(file.ContentType)
                ? "application/octet-stream"
                : file.ContentType;

            await storage.UploadObjectAsync(
                bucket: bucketName,
                objectName: objectName,
                contentType: contentType,
                source: stream
            );

            return $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media";
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
