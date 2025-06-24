using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.UserDetailOfAdminRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.UserDetailOfAdminService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AdminController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserForAdminController : Controller
    {
        private readonly IUserDetailOfAdminService _service;

        public UserForAdminController(IUserDetailOfAdminService service)
        {
            _service = service;
        }

        [HttpGet("GetUserByAccount/{accountId}")]
        public async Task<IActionResult> GetUserDetailByAccountId(int accountId)
        {
            var userDetail = await _service.GetUserDetailByAccountIdAsync(accountId);
            if (userDetail == null)
                return NotFound("User not found");

            return Ok(userDetail);
        }
    }
}
