using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.CompanyFollowingDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CompanyFollowingService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.CompanyFollowingController
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompanyFollowersController : ControllerBase
    {
        private readonly ICompanyFollowingService _service;

        public CompanyFollowersController(ICompanyFollowingService service)
        {
            _service = service;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CompanyFollowingDTO>>> GetFollowedCompaniesByUser(int userId)
        {
            var companies = await _service.GetFollowedCompaniesAsync(userId);
            return Ok(companies);
        }
    }
}
