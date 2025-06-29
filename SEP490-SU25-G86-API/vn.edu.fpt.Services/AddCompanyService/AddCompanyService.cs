using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AddCompanyRepository;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Services.AddCompanyService
{
    public class AddCompanyService : IAddCompanyService
    {
        private readonly IAddCompanyRepository _addcompanyRepository;

        public AddCompanyService(IAddCompanyRepository addcompanyRepository)
        {
            _addcompanyRepository = addcompanyRepository;
        }

        public async Task<Company> AddCompanyAsync(AddCompanyDTO dto, int employerId)
        {
            var company = new Company
            {
                CompanyName = dto.CompanyName,
                Description = dto.Description,
                Email = dto.Email,
                Address = dto.Address,
                Phone = dto.Phone,
                Website = dto.Website,
                TaxCode = dto.TaxCode,
                CreatedAt = DateTime.UtcNow
            };

            return await _addcompanyRepository.AddCompanyAsync(company);
        }
    }
}
