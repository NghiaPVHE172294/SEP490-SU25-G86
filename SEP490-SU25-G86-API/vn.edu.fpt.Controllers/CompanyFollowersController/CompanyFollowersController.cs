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
        public async Task<IActionResult> GetFollowedCompaniesByUser(int userId, int page = 1, int pageSize = 6)
        {
            var companies = await _service.GetFollowedCompaniesAsync(userId, page, pageSize);
            var total = await _service.CountFollowedCompaniesAsync(userId);
            return Ok(new { Companies = companies, Total = total });
        }

        [HttpGet("suggest/{userId}")]
        public async Task<IActionResult> GetSuggestedCompanies(int userId, int page = 1, int pageSize = 5)
        {
            var suggested = await _service.GetSuggestedCompaniesAsync(userId, page, pageSize);
            var total = await _service.CountSuggestedCompaniesAsync(userId);
            return Ok(new { Companies = suggested, Total = total });
        }
        [HttpDelete("{followId}")]
        public async Task<IActionResult> Unfollow(int followId)
        {
            var result = await _service.UnfollowCompanyAsync(followId);
            if (!result)
                return NotFound("Follow không tồn tại hoặc đã bị hủy trước đó.");

            return Ok("Unfollow thành công");
        }


    }
}
