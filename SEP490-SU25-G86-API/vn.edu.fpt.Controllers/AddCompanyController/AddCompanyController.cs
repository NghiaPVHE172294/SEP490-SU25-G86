using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.AddCompanyDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AddCompanyService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AddCompanyController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddCompanyController : ControllerBase
    {
        private readonly IAddCompanyService _addcompanyService;

        public AddCompanyController(IAddCompanyService addcompanyService)
        {
            _addcompanyService = addcompanyService;
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddCompany([FromBody] AddCompanyDTO dto)
        {
            var employerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var createdCompany = await _addcompanyService.AddCompanyAsync(dto, employerId);
            return Ok(createdCompany);
        }
    }
}
