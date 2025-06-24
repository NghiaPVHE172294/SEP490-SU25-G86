using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.CompanyController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _service;
        public CompanyController(ICompanyService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var company = await _service.GetCompanyDtoByIdAsync(id);
            if (company == null) return NotFound();
            return Ok(company);
        }
    }
}
