using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.AppliedJobDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AppliedJobServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AppliedJobController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppliedJobsController : ControllerBase
    {
        private readonly IAppliedJobService _appliedJobService;

        public AppliedJobsController(IAppliedJobService appliedJobService)
        {
            _appliedJobService = appliedJobService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<AppliedJobDTO>>> GetByUserId(int userId)
        {
            var result = await _appliedJobService.GetAppliedJobsByUserIdAsync(userId);
            return Ok(result);
        }
    }
} 