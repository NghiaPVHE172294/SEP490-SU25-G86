using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService;
using System.Security.Claims;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Drive.v3.Data;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.CVController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CvController : ControllerBase
    {
        private readonly ICvService _service;
        private readonly string _googleDriveFolderId = "1ZdRawzVPMq_8E4YlzvRKL_1YaRqvKX2N";
        private readonly string _serviceAccountJson = "sep490-su25-g86-cvmatcher-fc5755ffb723.json";
        public CvController(ICvService service)
        {
            _service = service;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyCvs()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng." });
            var userId = int.Parse(userIdClaim.Value);
            var result = await _service.GetAllByUserAsync(userId);
            return Ok(result);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCv([FromForm] AddCvDTO dto)
        {
            if (dto.File == null)
                return BadRequest(new { message = "Bạn chưa chọn file CV để upload." });
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng." });
            var userId = int.Parse(userIdClaim.Value);
            try
            {
                string fileUrl = await UploadFileToGoogleDrive(dto.File);
                await _service.AddAsync(userId, dto, fileUrl);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.ToString() });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCv(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized(new { message = "Không tìm thấy thông tin người dùng." });
            var userId = int.Parse(userIdClaim.Value);
            await _service.DeleteAsync(userId, id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCvDetail(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
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
                await request.UploadAsync();

                var uploadedFile = request.ResponseBody;
                if (uploadedFile == null)
                    throw new Exception("Không upload được file lên Google Drive. ResponseBody null.");
                if (string.IsNullOrEmpty(uploadedFile.Id))
                    throw new Exception("File upload lên Google Drive không có Id.");

                // Set quyền public cho file
                var permission = new Permission
                {
                    Type = "anyone",
                    Role = "reader"
                };
                await service.Permissions.Create(permission, uploadedFile.Id).ExecuteAsync();

                return uploadedFile.WebViewLink;
            }
        }
    }
}