using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService;
using SEP490_SU25_G86_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.JobController
{
    [ApiController]
    [Route("api/[controller]")]
    public class CvSubmissionsController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;
        private readonly SEP490_G86_CvMatchContext _context;
        public CvSubmissionsController(IJobPostService jobPostService, SEP490_G86_CvMatchContext context)
        {
            _jobPostService = jobPostService;
            _context = context;
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
            var result = await _jobPostService.GetCvSubmissionsByJobPostIdAsync(jobPostId);
            return Ok(result);
        }
    }
} 