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
        private readonly string _googleDriveFolderId = "1jlghm3ntLE6JDPcwJqA2tlVrmCVBVpYM";
        private readonly string _serviceAccountJson = "E:\\GithubProject_SEP490\\sep490-su25-g86-cvmatcher-2ad992eb4897.json";
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
                string fileUrl = await UploadFileToGoogleDrive(dto.File);
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

        // Upload file PDF lên Google Drive, trả về link xem file
        private async Task<string> UploadFileToGoogleDrive(IFormFile file)
        {
            var credential = GoogleCredential.FromFile(_serviceAccountJson)
                .CreateScoped(DriveService.Scope.Drive);

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "CVMatcher"
            });

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = file.FileName,
                Parents = new List<string> { _googleDriveFolderId }
            };

            using (var stream = file.OpenReadStream())
            {
                var request = service.Files.Create(fileMetadata, stream, file.ContentType);
                request.Fields = "id, webViewLink, webContentLink";
                request.SupportsAllDrives = true; // Hỗ trợ Drive dùng chung
                try
                {
                    var uploadResult = await request.UploadAsync();
                    Console.WriteLine($"[GoogleDrive] Upload status: {uploadResult.Status}, Exception: {uploadResult.Exception}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[GoogleDrive] Exception during upload: {ex}");
                    throw new Exception($"Google Drive upload exception: {ex.Message}", ex);
                }

                var uploadedFile = request.ResponseBody;
                if (uploadedFile == null)
                {
                    Console.WriteLine("[GoogleDrive] ResponseBody is null after upload. Possible cause: service account, folderId, or permission error.");
                    throw new Exception("Không upload được file lên Google Drive. ResponseBody null.");
                }
                if (string.IsNullOrEmpty(uploadedFile.Id))
                {
                    Console.WriteLine("[GoogleDrive] Uploaded file does not have an Id.");
                    throw new Exception("File upload lên Google Drive không có Id.");
                }

                // Set quyền public cho file
                var permission = new Google.Apis.Drive.v3.Data.Permission
                {
                    Type = "anyone",
                    Role = "reader"
                };
                var permissionRequest = service.Permissions.Create(permission, uploadedFile.Id);
                permissionRequest.SupportsAllDrives = true;
                await permissionRequest.ExecuteAsync();

                return uploadedFile.WebViewLink;
            }
        }
    }
}