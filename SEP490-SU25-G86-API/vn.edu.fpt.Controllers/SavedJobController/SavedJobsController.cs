using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.SavedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SavedJobService;
using Microsoft.AspNetCore.Authorization;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.SavedJobController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SavedJobsController : ControllerBase
    {
        private readonly ISavedJobService _savedJobService;

        public SavedJobsController(ISavedJobService savedJobService)
        {
            _savedJobService = savedJobService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<SavedJobDTO>>> GetByUserId(int userId)
        {
            var result = await _savedJobService.GetSavedJobsByUserIdAsync(userId);
            return Ok(result);
        }
    }
}
