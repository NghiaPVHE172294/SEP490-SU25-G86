using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService;
using System.Security.Claims;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Drive.v3.Data;
using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.CVController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CvController : ControllerBase
    {
        private readonly ICvService _service;

        private readonly SEP490_G86_CvMatchContext _context;

        public CvController(ICvService service, SEP490_G86_CvMatchContext context)

        {
            _service = service;
            _context = context;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyCvs()
        {
            var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (accountIdClaim == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin tài khoản." });
            var accountId = int.Parse(accountIdClaim.Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized(new { message = "Không tìm thấy người dùng tương ứng với tài khoản." });
            var result = await _service.GetAllByUserAsync(user.UserId);
            return Ok(result);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCv([FromForm] AddCvDTO dto)
        {
            if (dto.File == null)
                return BadRequest(new { message = "Bạn chưa chọn file CV để upload." });
            var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (accountIdClaim == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin tài khoản." });
            var accountId = int.Parse(accountIdClaim.Value);
            var user = await _context.Users.Include(u => u.Account).ThenInclude(a => a.Role).FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized(new { message = "Không tìm thấy người dùng tương ứng với tài khoản." });
            try
            {
                string fileUrl = await _service.UploadFileToFirebaseStorage(dto.File);
                string roleName = user.Account?.Role?.RoleName ?? string.Empty;
                await _service.AddAsync(user.UserId, roleName, dto, fileUrl);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết ra console để dễ debug
                Console.WriteLine($"[UploadCv] Exception: {ex}");
                return BadRequest(new { message = ex.ToString() });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCv(int id)
        {
            var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (accountIdClaim == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin tài khoản." });
            var accountId = int.Parse(accountIdClaim.Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized(new { message = "Không tìm thấy người dùng tương ứng với tài khoản." });
            try
            {
                await _service.DeleteAsync(user.UserId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCvDetail(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("rename/{cvId}")]
        public async Task<IActionResult> RenameCv(int cvId, [FromBody] string newName)
        {
            var cv = await _service.GetByIdAsync(cvId);
            if (cv == null) return NotFound();
            // Cập nhật tên
            await _service.UpdateCvNameAsync(cvId, newName);
            return Ok();
        }
    }
}