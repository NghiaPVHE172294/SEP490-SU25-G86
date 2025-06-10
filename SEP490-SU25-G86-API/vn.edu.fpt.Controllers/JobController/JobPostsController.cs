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
        public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetAllJobPosts()
        {
            var result = await _jobPostService.GetAllJobPostsAsync();
            return Ok(result);
        }
    }

}
