using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.EmploymentTypeDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.EmploymentTypeService;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.EmploymentTypesController
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmploymentTypesController : ControllerBase
    {
        private readonly IEmploymentTypeService _service;
        public EmploymentTypesController(IEmploymentTypeService service)
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
        public async Task<IActionResult> Add([FromBody] AddEmploymentTypeDTO dto)
        {
            var id = await _service.AddAsync(dto);
            return Ok(id);
        }
    }
}
