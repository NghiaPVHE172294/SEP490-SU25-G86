using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserService;
using System.Security.Claims;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (accountId == 0) return Unauthorized();

            var result = await _userService.GetUserProfileAsync(accountId);
            return result == null ? NotFound("Không tìm thấy hồ sơ.") : Ok(result);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDTO dto)
        {
            var accountId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (accountId == 0) return Unauthorized();

            var success = await _userService.UpdateUserProfileAsync(accountId, dto);
            return success ? Ok("Cập nhật thành công.") : BadRequest("Cập nhật thất bại.");
        }
    }
}
