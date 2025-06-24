using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobLevelDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobLevelService;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.JobLevelsController
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobLevelsController : ControllerBase
    {
        private readonly IJobLevelService _service;
        public JobLevelsController(IJobLevelService service)
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
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddJobLevelDTO dto)
        {
            var id = await _service.AddAsync(dto);
            return Ok(id);
        }
    }
}