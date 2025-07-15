using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AddCompanyService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AddCompanyController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly IInfoCompanyService _companyService;

        public CompaniesController(IInfoCompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpGet("me/{userId}")]
        public async Task<IActionResult> GetMyCompany(int userId)
        {
            var company = await _companyService.GetCompanyByUserIdAsync(userId);
            if (company == null) return NotFound();
            return Ok(company);
        }

        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompanyById(int companyId)
        {
            var company = await _companyService.GetCompanyByIdAsync(companyId);
            if (company == null) return NotFound();
            return Ok(company);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreateUpdateDTO dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var created = await _companyService.CreateCompanyAsync(userId, dto);
            if (!created) return BadRequest("User has already created a company.");
            return Ok("Company created successfully.");
        }


        [HttpPut("{companyId}")]
        public async Task<IActionResult> UpdateCompany(int companyId, [FromBody] CompanyCreateUpdateDTO dto)
        {
            var updated = await _companyService.UpdateCompanyAsync(companyId, dto);
            if (!updated) return NotFound("Company not found.");
            return Ok("Company updated successfully.");
        }
    }
}
