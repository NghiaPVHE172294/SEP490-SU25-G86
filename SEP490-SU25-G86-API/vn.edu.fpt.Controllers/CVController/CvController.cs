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
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService;
using System.Text;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.CVController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CvController : ControllerBase
    {
        private readonly ICvService _service;
        private readonly ICvParsingService _cvParsing;//thêm mới để hỗ trợ parsing CV
        private readonly SEP490_G86_CvMatchContext _context;
        private readonly IWebHostEnvironment _env;

        public CvController(ICvService service, ICvParsingService cvParsing, SEP490_G86_CvMatchContext context, IWebHostEnvironment env)
        {
            _service = service;
            _cvParsing = cvParsing;// thêm mới để hỗ trợ parsing CV
            _context = context;
            _env = env;
        }
        // Helper: đọc prompt từ file, có cache để tránh đọc file mỗi request
        private static string? _cachedCvPrompt;
        private string GetCvPrompt()
        {
            if (!string.IsNullOrEmpty(_cachedCvPrompt)) return _cachedCvPrompt;

            var path = Path.Combine(_env.ContentRootPath, "LogAPI_AI", "GeminiPromtToParsedData.txt");
            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException($"Không tìm thấy file prompt: {path}");

            // Đọc UTF-8 để không lỗi tiếng Việt
            _cachedCvPrompt = System.IO.File.ReadAllText(path, Encoding.UTF8);
            return _cachedCvPrompt!;
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
        public async Task<IActionResult> UploadCv([FromForm] AddCvDTO dto, CancellationToken ct = default)
        {
            if (dto.File == null)
                return BadRequest(new { message = "Bạn chưa chọn file CV để upload." });

            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin tài khoản." });

            var accountId = int.Parse(accountIdClaim.Value);
            var user = await _context.Users.Include(u => u.Account).ThenInclude(a => a.Role)
                                           .FirstOrDefaultAsync(u => u.AccountId == accountId, ct);
            if (user == null)
                return Unauthorized(new { message = "Không tìm thấy người dùng tương ứng với tài khoản." });

            try
            {
                // Gọi GetCvPrompt()
                var prompt = GetCvPrompt();

                // 1) Upload file -> Firebase
                string fileUrl = await _service.UploadFileToFirebaseStorage(dto.File, user.UserId);

                // 2) Tạo record trong bảng CVs -> nhận cvId
                string roleName = user.Account?.Role?.RoleName ?? string.Empty;
                int cvId = await _service.AddAsync(user.UserId, roleName, dto, fileUrl);

                // 3) Parse CV đã lưu -> ghi bảng CVParsedData
                var parsed = await _cvParsing.ParseAndSaveFromUrlAsync(cvId, fileUrl, prompt, ct);

                return Ok(new
                {
                    CvId = cvId,
                    FileUrl = fileUrl,
                    CvParsedDataId = parsed.CvparsedDataId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UploadCv] Exception: {ex}");
                return BadRequest(new { message = ex.Message });
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