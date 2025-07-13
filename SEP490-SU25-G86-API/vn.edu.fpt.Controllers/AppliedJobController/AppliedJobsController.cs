using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AppliedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AppliedJobDTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AppliedJobController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppliedJobsController : ControllerBase
    {
        private readonly IAppliedJobService _appliedJobService;
        private readonly ICvService _cvService;

        public AppliedJobsController(IAppliedJobService appliedJobService, ICvService cvService)
        {
            _appliedJobService = appliedJobService;
            _cvService = cvService;
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
            var submission = new SEP490_SU25_G86_API.Models.Cvsubmission
            {
                CvId = req.CvId,
                JobPostId = req.JobPostId,
                SubmittedByUserId = req.CandidateId,
                SubmissionDate = DateTime.UtcNow,
                IsDelete = false,
                SourceType = "EXISTING"
            };
            await _appliedJobService.AddSubmissionAsync(submission);
            return Ok(new { message = "Ứng tuyển thành công" });
        }

        [HttpPost("apply-upload")]
        public async Task<IActionResult> ApplyWithNewCv([FromForm] ApplyUploadCvDTO req)
        {
            if (req.File == null || req.File.Length == 0)
                return BadRequest("Vui lòng chọn file CV");
            // Upload file lên Google Drive (dùng lại logic của CvController)
            string fileUrl = await _cvService.UploadFileToGoogleDrive(req.File);
            var cv = new SEP490_SU25_G86_API.Models.Cv
            {
                CandidateId = req.CandidateId,
                FileUrl = fileUrl,
                Notes = req.Notes,
                UploadDate = DateTime.UtcNow,
                IsDelete = false,
                UploadByUserId = req.CandidateId,
                Cvname = req.CVName
            };
            int cvId = await _appliedJobService.AddCvAndGetIdAsync(cv);
            // Tạo bản ghi ứng tuyển vào jobpost (CvSubmission)
            var submission = new SEP490_SU25_G86_API.Models.Cvsubmission
            {
                CvId = cvId,
                JobPostId = req.JobPostId,
                SubmittedByUserId = req.CandidateId,
                SubmissionDate = DateTime.UtcNow,
                IsDelete = false,
                SourceType = "UPLOAD"
            };
            await _appliedJobService.AddSubmissionAsync(submission);
            return Ok(new { message = "Ứng tuyển thành công" });
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