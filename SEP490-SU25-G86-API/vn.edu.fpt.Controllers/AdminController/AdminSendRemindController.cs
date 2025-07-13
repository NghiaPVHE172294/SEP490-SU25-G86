using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.EmailDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.RemindService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AdminController
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    [AllowAnonymous]
    public class AdminSendRemindController : Controller
    {
        private readonly IRemindService _remindService;
        public AdminSendRemindController(IRemindService remindService)
        {
            _remindService = remindService;
        }

        [HttpPost("sendRemind")]
        public async Task<IActionResult> SendReminder([FromBody] ReminderEmailRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail) || string.IsNullOrWhiteSpace(request.Message))
                return BadRequest("Email và nội dung không được để trống.");

            await _remindService.SendReminderAsync(request);
            return Ok("Đã gửi lời nhắc thành công.");
        }
    }
}
