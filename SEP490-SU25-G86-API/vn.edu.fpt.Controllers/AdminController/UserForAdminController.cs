using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserDetailOfAdminRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.BanUserService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserDetailOfAdminService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AdminController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserForAdminController : Controller
    {
        private readonly IUserDetailOfAdminService _service;
        private readonly IBanUserService _banUserService;

        public UserForAdminController(IUserDetailOfAdminService service, IBanUserService banUserService)
        {
            _service = service;
            _banUserService = banUserService;
        }

        [HttpGet("GetUserByAccount/{accountId}")]
        public async Task<IActionResult> GetUserDetailByAccountId(int accountId)
        {
            var userDetail = await _service.GetUserDetailByAccountIdAsync(accountId);
            if (userDetail == null)
                return NotFound("User not found");

            return Ok(userDetail);
        }

        [HttpPost("BanUser/{userId}")]
        public async Task<IActionResult> BanUser(int userId)
        {
            var result = await _banUserService.BanUserAsync(userId);
            if (!result)
            {
                return NotFound(new { message = "User not found!" });
            }

            return Ok(new { message = "User banned successfully!" });
        }

        [HttpPost("UnbanUser/{userId}")]
        public async Task<IActionResult> UnbanUser(int userId)
        {
            var result = await _banUserService.UnbanUserAsync(userId);
            if (!result)
            {
                return NotFound(new { message = "User not found!" });
            }

            return Ok(new { message = "User unbanned successfully!" });
        }
    }
}
