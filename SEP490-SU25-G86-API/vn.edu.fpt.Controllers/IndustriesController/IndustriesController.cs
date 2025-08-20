using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.IndustryDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.IndustryDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.IndustryService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.IndustriesController
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndustriesController : ControllerBase
    {
        private readonly IIndustryService _industryService;

        public IndustriesController(IIndustryService industryService)
        {
            _industryService = industryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Industry>>> GetIndustries()
        {
            var industries = await _industryService.GetAllIndustriesAsync();
            return Ok(industries.Select(i => new {
                i.IndustryId,
                i.IndustryName
            }));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] AddIndustryDTO dto)
        {
            var id = await _industryService.AddAsync(dto);
            return Ok(id);
        }
        [HttpGet("with-jobcount")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<IndustryWithJobCountDTO>>> GetIndustriesWithJobCount([FromQuery] int page = 1, [FromQuery] int pageSize = 8)
        {
            var industries = await _industryService.GetIndustriesWithJobCount(page, pageSize);
            return Ok(industries);
        }
    }
}
