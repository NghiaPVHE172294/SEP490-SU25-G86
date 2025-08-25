using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminAccoutServices;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminAccountController : Controller
    {
        private readonly IAccountListService _accountService;

        public AdminAccountController(IAccountListService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
        [FromQuery] string? role,
        [FromQuery] string? name,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize is < 1 or > 100 ? 10 : pageSize; // chặn pageSize quá lớn

            var paged = await _accountService.GetAccountsAsync(role, name, pageNumber, pageSize, ct);
            return Ok(paged);
        }

        [HttpGet("role/{roleName}")]
        public Task<IActionResult> GetByRole(string roleName, [FromQuery] string? name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default)
        => Task.FromResult<IActionResult>(RedirectToAction(nameof(GetAll), new { role = roleName, name, pageNumber, pageSize }));
    }
}
