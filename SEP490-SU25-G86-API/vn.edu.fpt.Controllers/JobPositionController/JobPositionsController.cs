using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPositionDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPositionService;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.JobPositionController
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPositionsController : ControllerBase
    {
        private readonly IJobPositionService _service;
        public JobPositionsController(IJobPositionService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("by-industry/{industryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIndustryId(int industryId)
        {
            var list = await _service.GetByIndustryIdAsync(industryId);
            return Ok(list);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddJobPositionDTO dto)
        {
            var id = await _service.AddAsync(dto);
            return Ok(id);
        }
    }
} 