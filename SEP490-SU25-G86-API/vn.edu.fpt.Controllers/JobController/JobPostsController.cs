using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.JobController
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostsController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;

        public JobPostsController(IJobPostService jobPostService)
        {
            _jobPostService = jobPostService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetAllJobPosts()
        {
            var result = await _jobPostService.GetAllJobPostsAsync();
            return Ok(result);
        }

        [HttpGet("employer/{employerId}")]
        [Authorize(Roles = "EMPLOYER")]
        public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetByEmployerId(int employerId)
        {
            var result = await _jobPostService.GetByEmployerIdAsync(employerId);
            return Ok(result);
        }

    }

}
