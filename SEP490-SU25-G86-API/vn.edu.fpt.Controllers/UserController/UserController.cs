using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CvService;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserService;
using System.Security.Claims;
using static SEP490_SU25_G86_API.vn.edu.fpt.DTOs.UserDTO.UserFollow;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.UserController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpPost("follow")]
        public async Task<IActionResult> FollowCompany([FromBody] FollowRequest request)
        {
            var result = await _userService.FollowCompanyAsync(request.UserId, request.CompanyId);
            if (result)
                return Ok("Theo dõi công ty thành công.");
            return BadRequest("Bạn đã theo dõi công ty này rồi.");
        }

        [HttpPost("block")]
        public async Task<IActionResult> BlockCompany([FromBody] BlockRequest request)
        {
            var result = await _userService.BlockCompanyAsync(request.UserId, request.CompanyId, request.Reason);
            if (result)
                return Ok("Chặn công ty thành công.");
            return BadRequest("Bạn đã chặn công ty này rồi.");
        }
    }
}
