using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.SalaryRangeService;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.SalaryRangeDTO;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.SalaryRangeController
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalaryRangesController : ControllerBase
    {
        private readonly ISalaryRangeService _service;
        public SalaryRangesController(ISalaryRangeService service)
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
        public async Task<IActionResult> Add([FromBody] AddSalaryRangeDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var id = await _service.AddAsync(dto);
            return Ok(id);
        }
    }
} 