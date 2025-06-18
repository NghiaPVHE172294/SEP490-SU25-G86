using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.Models;
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
        public async Task<ActionResult<IEnumerable<Industry>>> GetIndustries()
        {
            var industries = await _industryService.GetAllIndustriesAsync();
            return Ok(industries.Select(i => new {
                i.IndustryId,
                i.IndustryName
            }));
        }
    }
}
