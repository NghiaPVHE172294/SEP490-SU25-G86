using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.NotificationDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.NotificationService;


namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.NotificationController
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _svc;
        public NotificationsController(INotificationService svc) => _svc = svc;

        [HttpPost]
        public async Task<ActionResult<NotificationResponse>> Send([FromBody] CreateNotificationRequest req, CancellationToken ct)
        {
            var result = await _svc.SendAsync(req, ct);
            return CreatedAtAction(nameof(GetLatest), new { userId = result.ReceiverUserId }, result);
        }

        [HttpGet("{userId:long}")]
        public async Task<ActionResult<IReadOnlyList<NotificationResponse>>> GetLatest([FromRoute] long userId, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var list = await _svc.GetLatestAsync(userId, take, ct);
            return Ok(list);
        }

        [HttpPut("{id:long}/read")]
        public async Task<IActionResult> MarkRead([FromRoute] long id, [FromQuery] long userId, CancellationToken ct)
        {
            await _svc.MarkReadAsync(id, userId, ct);
            return NoContent();
        }
    }
}
