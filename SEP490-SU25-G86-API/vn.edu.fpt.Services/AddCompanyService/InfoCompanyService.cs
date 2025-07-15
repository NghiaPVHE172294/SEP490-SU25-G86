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

        // Lấy công ty theo tài khoản (AccountId)
        public async Task<CompanyDetailDTO?> GetCompanyByAccountIdAsync(int accountId)
        {
            var company = await _repository.GetByAccountIdAsync(accountId);
            if (company == null) return null;

            return MapToDetailDto(company);
        }

        // Lấy công ty theo ID
        public async Task<CompanyDetailDTO?> GetCompanyByIdAsync(int companyId)
        {
            var company = await _repository.GetByIdAsync(companyId);
            if (company == null) return null;

            return MapToDetailDto(company);
        }

        // Tạo mới công ty (check trùng trước)
        public async Task<bool> CreateCompanyAsync(int accountId, CompanyCreateUpdateDTO dto)
        {
            // Nếu đã có công ty với accountId => không cho tạo
            var existing = await _repository.GetByAccountIdAsync(accountId);
            if (existing != null) return false;

            // Kiểm tra trùng thông tin
            var isDuplicate = await _repository.IsDuplicateCompanyAsync(dto);
            if (isDuplicate)
                throw new Exception("Thông tin công ty bị trùng (Tên, MST, Email, SĐT hoặc Website).");

            var company = new Company
            {
                CompanyName = dto.CompanyName,
                TaxCode = dto.TaxCode,
                IndustryId = dto.IndustryId,
                Email = dto.Email,
                Address = dto.Address,
                Description = dto.Description,
                Website = dto.Website,
                CompanySize = dto.CompanySize,
                Phone = dto.Phone,
                LogoUrl = dto.LogoUrl,
                CreatedByUserId = accountId, // Gán accountId vào trường CreatedByUserId
                CreatedAt = DateTime.UtcNow,
                IsDelete = false
            };

            await _repository.CreateAsync(company);
            return true;
        }

        // Cập nhật công ty
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
            company.Website = dto.Website;
            company.CompanySize = dto.CompanySize;
            company.Phone = dto.Phone;
            company.LogoUrl = dto.LogoUrl;

            await _repository.UpdateAsync(company);
            return true;
        }

        // Map từ entity sang DTO
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
    }
}
