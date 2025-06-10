using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("homepage")]
        public async Task<IActionResult> GetHomeJobs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 9,
        [FromQuery] string? region = null) // ← thêm region filter
        {
            var jobs = await _jobPostService.GetPagedJobPostsAsync(page, pageSize, region);

            return Ok(new
            {
                TotalItems = jobs.Item2,
                Jobs = jobs.Item1
            });
        }
    }
}
