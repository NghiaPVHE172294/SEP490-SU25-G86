using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService;
using SEP490_SU25_G86_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService;
using Google.Api;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AdminDashboardDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CvDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.JobController
{
    [ApiController]
    [Route("api/[controller]")]
    public class CvSubmissionsController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;
        private readonly SEP490_G86_CvMatchContext _context;
        private readonly ICvSubmissionService _service;
        public CvSubmissionsController(IJobPostService jobPostService, SEP490_G86_CvMatchContext context, ICvSubmissionService service)
        {
            _jobPostService = jobPostService;
            _context = context;
            _service = service;
        }

        [HttpGet("jobpost/{jobPostId}")]
        [Authorize]
        public async Task<IActionResult> GetCvSubmissionsByJobPostId(int jobPostId)
        {
            // Lấy accountId từ token
            var accountIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(accountIdStr, out int accountId))
                return Unauthorized("Không xác định được tài khoản.");
            // Lấy user
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");
            // Lấy jobpost
            var jobPost = await _context.JobPosts.FirstOrDefaultAsync(j => j.JobPostId == jobPostId);
            if (jobPost == null)
                return NotFound("Không tìm thấy job post.");
            if (jobPost.EmployerId != user.UserId)
                return Unauthorized("Bạn không có quyền xem CV submissions của job post này.");
            // Trả về danh sách submissions có Status và TotalScore
            var result = await _jobPostService.GetCvSubmissionsByJobPostIdAsync(jobPostId);
            return Ok(result);
        }

        [HttpPatch("recruiter-note")]
        [Authorize]
        public async Task<IActionResult> UpdateRecruiterNote([FromBody] CVSubRecruiterNoteDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RecruiterNote))
                return BadRequest("RecruiterNote must not be empty.");

            var result = await _service.UpdateRecruiterNoteAsync(dto.SubmissionId, dto.RecruiterNote);
            if (!result)
                return NotFound("Submission not found.");

            return Ok("Recruiter note updated successfully.");
        }

        [HttpPut("{id}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateSubmissionStatus(int id, [FromBody] string newStatus)
        {
            var submission = await _context.Cvsubmissions.FirstOrDefaultAsync(c => c.SubmissionId == id && !c.IsDelete);
            if (submission == null)
                return NotFound("Không tìm thấy submission.");
            submission.Status = newStatus;
            _context.Cvsubmissions.Update(submission);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpPut("withdraw/{id}")]
        [Authorize]
        public async Task<IActionResult> WithdrawSubmission(int id)
        {
            var submission = await _context.Cvsubmissions.FirstOrDefaultAsync(c => c.SubmissionId == id && !c.IsDelete);
            if (submission == null)
                return NotFound("Không tìm thấy submission.");
            submission.Status = "Hồ sơ đã rút";
            submission.IsDelete = true;
            _context.Cvsubmissions.Update(submission);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
} 