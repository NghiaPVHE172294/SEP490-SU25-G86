using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AppliedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AppliedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.NotificationService;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AppliedJobController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppliedJobsController : ControllerBase
    {
        private readonly IAppliedJobService _appliedJobService;
        private readonly ICvService _cvService;
        private readonly ICvParsingService _cvParsing;
        private readonly SEP490_G86_CvMatchContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly INotificationService _notificationService;

        public AppliedJobsController(
            IAppliedJobService appliedJobService,
            ICvService cvService,
            ICvParsingService cvParsing,
            SEP490_G86_CvMatchContext context,
            IWebHostEnvironment env,
            INotificationService notificationService)
        {
            _appliedJobService = appliedJobService;
            _cvService = cvService;
            _cvParsing = cvParsing;
            _context = context;
            _env = env;
            _notificationService = notificationService;
        }

        // ===== Prompt từ file (cache để tránh đọc lặp) =====
        private static string? _cachedCvPrompt;
        private string GetCvPrompt()
        {
            if (!string.IsNullOrEmpty(_cachedCvPrompt)) return _cachedCvPrompt!;

            var path = Path.Combine(_env.ContentRootPath, "LogAPI_AI", "GeminiPromtToParsedData.txt");
            if (!System.IO.File.Exists(path))
                throw new FileNotFoundException($"Không tìm thấy file prompt: {path}");

            _cachedCvPrompt = System.IO.File.ReadAllText(path, Encoding.UTF8);
            return _cachedCvPrompt!;
        }


        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AppliedJobDTO>>> GetByUserId(int userId)
        {
            var result = await _appliedJobService.GetAppliedJobsByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPost("apply-existing")]
        public async Task<IActionResult> ApplyWithExistingCv([FromBody] ApplyExistingCvDTO req)
        {
            // Check if user has already applied
            if (await _appliedJobService.HasUserAppliedToJobAsync(req.CandidateId, req.JobPostId))
            {
                return BadRequest(new { message = "Bạn đã ứng tuyển công việc này rồi!" });
            }
            var submission = new SEP490_SU25_G86_API.Models.Cvsubmission
            {
                CvId = req.CvId,
                JobPostId = req.JobPostId,
                SubmittedByUserId = req.CandidateId,
                SubmissionDate = DateTime.UtcNow,
                IsDelete = false,
                SourceType = "EXISTING",
                Status = "Đã ứng tuyển"
            };
            await _appliedJobService.AddSubmissionAsync(submission);

            // Gửi notification cho ứng viên
            await _notificationService.SendAsync(new SEP490_SU25_G86_API.vn.edu.fpt.DTOs.NotificationDTO.CreateNotificationRequest(
                ReceiverUserId: req.CandidateId,
                Content: "Bạn đã nộp hồ sơ ứng tuyển thành công, vui lòng truy cập vào trang 'việc làm đã ứng tuyển' để cập nhật trạng thái hồ sơ.",
                TargetUrl: "https://localhost:7283/Candidate/AppliedJobs"
            ));

            return Ok(new { message = "Ứng tuyển thành công" });
        }

        [HttpPost("apply-upload")]
        public async Task<IActionResult> ApplyWithNewCv([FromForm] ApplyUploadCvDTO req, CancellationToken ct)
        {
            if (req.File == null || req.File.Length == 0)
                return BadRequest("Vui lòng chọn file CV");

            // chỉ nhận PDF/DOCX
            var ext = Path.GetExtension(req.File.FileName).ToLowerInvariant();
            if (ext != ".pdf" && ext != ".docx")
                return BadRequest("Chỉ hỗ trợ CV dạng PDF hoặc DOCX.");

            // Không cho nộp trùng
            if (await _appliedJobService.HasUserAppliedToJobAsync(req.CandidateId, req.JobPostId))
                return BadRequest(new { message = "Bạn đã ứng tuyển công việc này rồi!" });

            await using var tx = await _context.Database.BeginTransactionAsync(ct);
            try
            {
                // 1) Upload file lên Firebase
                string fileUrl = await _cvService.UploadFileToFirebaseStorage(req.File, req.CandidateId);

                // 2) Tạo record trong bảng CVs
                var cv = new SEP490_SU25_G86_API.Models.Cv
                {
                    CandidateId = req.CandidateId,
                    UploadByUserId = req.CandidateId,
                    FileUrl = fileUrl,
                    UploadDate = DateTime.UtcNow,
                    IsDelete = false,
                    Cvname = string.IsNullOrWhiteSpace(req.CVName) ? Path.GetFileName(req.File.FileName) : req.CVName,
                    Notes = req.Notes
                };
                int cvId = await _appliedJobService.AddCvAndGetIdAsync(cv);

                // 3) Tạo record Submission
                var submission = new SEP490_SU25_G86_API.Models.Cvsubmission
                {
                    CvId = cvId,
                    JobPostId = req.JobPostId,
                    SubmittedByUserId = req.CandidateId,
                    SubmissionDate = DateTime.UtcNow,
                    IsDelete = false,
                    SourceType = "UPLOAD",
                    Status = "Đã ứng tuyển"
                };
                await _appliedJobService.AddSubmissionAsync(submission);

                // 4) Parse CV đã lưu -> ghi bảng CVParsedData
                var prompt = GetCvPrompt();
                var parsed = await _cvParsing.ParseAndSaveFromUrlAsync(cvId, fileUrl, prompt, ct);

                await tx.CommitAsync(ct);

                // Gửi notification cho ứng viên
                await _notificationService.SendAsync(new SEP490_SU25_G86_API.vn.edu.fpt.DTOs.NotificationDTO.CreateNotificationRequest(
                    ReceiverUserId: req.CandidateId,
                    Content: "Bạn đã nộp hồ sơ ứng tuyển thành công, vui lòng truy cập vào trang 'việc làm đã ứng tuyển' để cập nhật trạng thái hồ sơ.",
                    TargetUrl: "https://localhost:7283/Candidate/AppliedJobs"
                ));

                return Ok(new
                {
                    message = "Ứng tuyển thành công",
                    CvId = cvId,
                    FileUrl = fileUrl,
                    SubmissionId = submission.SubmissionId,
                    CvParsedDataId = parsed.CvparsedDataId
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(ct);
                Console.WriteLine($"[ApplyWithNewCv] Exception: {ex}");
                // Nếu muốn không rollback khi parse fail, có thể commit trước, rồi catch riêng lỗi parse
                return BadRequest(new { message = "Ứng tuyển thất bại: " + ex.Message });
            }
        }


        [HttpPut("update-cv")]
        public async Task<IActionResult> UpdateAppliedCv([FromBody] UpdateAppliedCvDTO req)
        {
            if (req == null)
                return BadRequest(new { message = "Dữ liệu yêu cầu không hợp lệ." });
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return BadRequest(new { message = "Dữ liệu không hợp lệ: " + string.Join(", ", errors) });
            }
            
            var updated = await _appliedJobService.UpdateAppliedCvAsync(req.SubmissionId, req.NewCvId, req.UserId);
            if (!updated)
                return BadRequest(new { message = "Không thể cập nhật CV cho đơn ứng tuyển này. Có thể đơn ứng tuyển không tồn tại hoặc bạn không có quyền cập nhật." });
            return Ok(new { message = "Cập nhật CV thành công." });
        }

        [HttpDelete("withdraw/{submissionId}")]
        public async Task<IActionResult> WithdrawApplication(int submissionId, [FromQuery] int userId)
        {
            var ok = await _appliedJobService.WithdrawApplicationAsync(submissionId, userId);
            if (!ok)
                return BadRequest(new { message = "Không thể rút đơn ứng tuyển này." });
            return Ok(new { message = "Rút đơn ứng tuyển thành công." });
        }

        public class ApplyExistingCvRequest
        {
            public int JobPostId { get; set; }
            public int CvId { get; set; }
            public int CandidateId { get; set; }
        }
        public class ApplyUploadCvRequest
        {
            public int JobPostId { get; set; }
            public int CandidateId { get; set; }
            public string? Notes { get; set; }
            public Microsoft.AspNetCore.Http.IFormFile File { get; set; }
        }
    }
} 