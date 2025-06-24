using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.ExperienceLevelDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.ExperienceLevelService;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.ExperienceLevelsController
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExperienceLevelsController : ControllerBase
    {
        private readonly IExperienceLevelService _service;
        public ExperienceLevelsController(IExperienceLevelService service)
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
        public async Task<IActionResult> Add([FromBody] AddExperienceLevelDTO dto)
        {
            var id = await _service.AddAsync(dto);
            return Ok(id);
        }
    }
}