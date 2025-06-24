using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AdminController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            return Ok(new { message = "Chào mừng Admin! Đây là dashboard dành cho Admin." });
        }
    }
} 