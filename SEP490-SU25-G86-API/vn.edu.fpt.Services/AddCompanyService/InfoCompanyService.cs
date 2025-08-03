using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AddCompanyRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AddCompanyService
{
    public class InfoCompanyService : IInfoCompanyService
    {
        private readonly IInfoCompanyRepository _repository;

        public InfoCompanyService(IInfoCompanyRepository repository)
        {
            _repository = repository;
        }

        public async Task<CompanyDetailDTO?> GetCompanyByAccountIdAsync(int accountId)
        {
            var company = await _repository.GetByAccountIdAsync(accountId);
            if (company == null) return null;
            return MapToDetailDto(company);
        }

        public async Task<CompanyDetailDTO?> GetCompanyByIdAsync(int companyId)
        {
            var company = await _repository.GetByIdAsync(companyId);
            if (company == null) return null;
            return MapToDetailDto(company);
        }

        private string? NormalizeUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            url = url.Trim();
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                return "https://" + url;
            return url;
        }

        public async Task<bool> CreateCompanyAsync(int accountId, CompanyCreateUpdateDTO dto)
        {
            var existing = await _repository.GetByAccountIdAsync(accountId);
            if (existing != null) return false;

            var isDuplicate = await _repository.IsDuplicateCompanyAsync(dto);
            if (isDuplicate)
                throw new Exception("Thông tin công ty bị trùng (Tên, MST, Email, SĐT hoặc Website).");

            string? firebaseLogoUrl = null;
            if (dto.LogoFile != null)
            {
                firebaseLogoUrl = await UploadLogoToFirebaseStorage(dto.LogoFile, accountId);
            }

            var company = new Company
            {
                CompanyName = dto.CompanyName,
                TaxCode = dto.TaxCode,
                IndustryId = dto.IndustryId,
                Email = dto.Email,
                Address = dto.Address,
                Description = dto.Description,
                Website = NormalizeUrl(dto.Website),
                CompanySize = dto.CompanySize,
                Phone = dto.Phone,
                LogoUrl = firebaseLogoUrl,
                CreatedByUserId = accountId,
                CreatedAt = DateTime.UtcNow,
                IsDelete = false
            };

            await _repository.CreateAsync(company);
            return true;
        }

        public async Task<bool> UpdateCompanyAsync(int companyId, CompanyCreateUpdateDTO dto)
        {
            var company = await _repository.GetByIdAsync(companyId);
            if (company == null) return false;

            company.CompanyName = dto.CompanyName;
            company.TaxCode = dto.TaxCode;
            company.IndustryId = dto.IndustryId;
            company.Email = dto.Email;
            company.Address = dto.Address;
            company.Description = dto.Description;
            company.Website = NormalizeUrl(dto.Website);
            company.CompanySize = dto.CompanySize;
            company.Phone = dto.Phone;

            if (dto.LogoFile != null)
            {
                // Có thể thêm xóa logo cũ ở đây nếu muốn
                company.LogoUrl = await UploadLogoToFirebaseStorage(dto.LogoFile, company.CreatedByUserId);

            }

            await _repository.UpdateAsync(company);
            return true;
        }

        private CompanyDetailDTO MapToDetailDto(Company c) => new()
        {
            CompanyId = c.CompanyId,
            CompanyName = c.CompanyName,
            TaxCode = c.TaxCode,
            Email = c.Email,
            Address = c.Address,
            Description = c.Description,
            Website = c.Website,
            CompanySize = c.CompanySize,
            Phone = c.Phone,
            LogoUrl = c.LogoUrl,
            IndustryId = c.IndustryId,
            IndustryName = c.Industry?.IndustryName ?? "",
            CreatedAt = c.CreatedAt
        };

        private async Task<string> UploadLogoToFirebaseStorage(IFormFile file, int userId)
        {
            string firebaseCredentialsPath = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS")
                ?? "D:\\FPTU\\SEP490_SUMMER25_G86\\sep490-su25-g86-cvmatcher-25bbfc6aba06.json";

            string bucketName = Environment.GetEnvironmentVariable("FIREBASE_BUCKET")
                ?? "sep490-su25-g86-cvmatcher.firebasestorage.app";

            string folderName = "Image_storage/CompanyAvatar";

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
            string objectName = $"{folderName}/{userId}_{timestamp}_{file.FileName}";

            var obj = await storage.UploadObjectAsync(
                bucket: bucketName,
                objectName: objectName,
                contentType: file.ContentType,
                source: stream
            );

            return $"https://firebasestorage.googleapis.com/v0/b/{bucketName}/o/{Uri.EscapeDataString(objectName)}?alt=media";
        }
    }
}
