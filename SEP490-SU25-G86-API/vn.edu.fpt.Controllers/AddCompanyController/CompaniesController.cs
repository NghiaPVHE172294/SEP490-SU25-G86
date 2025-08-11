using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("me")]
        public async Task<IActionResult> GetMyCompany()
        {
            var accountIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdStr) || !int.TryParse(accountIdStr, out int accountId))
                return Unauthorized();

            var company = await _companyService.GetCompanyByAccountIdAsync(accountId);
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
        [RequestSizeLimit(10_000_000)] // Optional: Limit file size (10MB)
        public async Task<IActionResult> CreateCompany([FromForm] CompanyCreateUpdateDTO dto)
        {
            var accountIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdStr) || !int.TryParse(accountIdStr, out int accountId))
                return Unauthorized();

            try
            {
                var created = await _companyService.CreateCompanyAsync(accountId, dto);
                if (!created)
                    return BadRequest("Tài khoản này đã tạo công ty.");

                return Ok("Tạo công ty thành công.");
            }
            catch (FileNotFoundException ex)
            {
                // Lỗi file credentials không tồn tại
                return StatusCode(500, $"Lỗi cấu hình: {ex.Message}");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("trùng") || ex.Message.Contains("upload ảnh"))
                {
                    // Lỗi nghiệp vụ hoặc upload ảnh
                    return BadRequest(ex.Message);
                }
                else
                {
                    // Lỗi không mong muốn
                    return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
                }
            }
        }

        [HttpPut("{companyId}")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UpdateCompany(int companyId, [FromForm] CompanyCreateUpdateDTO dto)
        {
            try
            {
                var updated = await _companyService.UpdateCompanyAsync(companyId, dto);
                if (!updated)
                    return NotFound("Không tìm thấy công ty để cập nhật.");

                return Ok("Cập nhật công ty thành công.");
            }
            catch (FileNotFoundException ex)
            {
                // Lỗi file credentials không tồn tại
                return StatusCode(500, $"Lỗi cấu hình: {ex.Message}");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("trùng") || ex.Message.Contains("upload ảnh"))
                {
                    // Lỗi nghiệp vụ hoặc upload ảnh
                    return BadRequest(ex.Message);
                }
                else
                {
                    // Lỗi không mong muốn
                    return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
                }
            }
        }
    }
}
