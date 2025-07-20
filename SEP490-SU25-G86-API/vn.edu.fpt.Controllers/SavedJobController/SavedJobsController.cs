using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SavedJobService;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.SavedJobDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.SavedJobController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SavedJobsController : ControllerBase
    {
        private readonly ISavedJobService _service;

        public SavedJobsController(ISavedJobService service)
        {
            _service = service;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<SavedJobDTO>>> GetByUserId(int userId)
        {
            var result = await _service.GetSavedJobsByUserIdAsync(userId);
            return Ok(result);
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveJob([FromBody] SaveJobRequest request)
        {
            var success = await _service.SaveJobAsync(request.UserId, request.JobPostId);
            if (!success)
                return BadRequest("Job already saved.");

            return Ok(new { message = "Job saved successfully." });
        }

        [HttpGet("check")]
        public async Task<IActionResult> IsSaved([FromQuery] int userId, [FromQuery] int jobPostId)
        {
            var isSaved = await _service.IsJobSavedAsync(userId, jobPostId);
            return Ok(new { isSaved });
        }

        [HttpDelete("{saveJobId}")]
        public async Task<IActionResult> DeleteSavedJob(int saveJobId)
        {
            var success = await _service.DeleteSavedJobAsync(saveJobId);
            if (!success) return NotFound();

            return NoContent();
        }
    }

    public class SaveJobRequest
    {
        public int UserId { get; set; }
        public int JobPostId { get; set; }
    }
}
