using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminAccoutServices;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers
{   
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize(Roles = "Admin")]
    public class AdminAccountController : Controller
    {
        private readonly IAccountListService _accountService;

        public AdminAccountController(IAccountListService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts([FromQuery] string? name)
        {
            var accounts = await _accountService.GetAllAccountsAsync();

            if (!string.IsNullOrWhiteSpace(name))
            {
                accounts = accounts
                            .Where(a => a.FullName != null && a.FullName.Contains(name, StringComparison.OrdinalIgnoreCase))
                            .ToList();
            }

            return Ok(accounts);
        }

        [HttpGet("role/{roleName}")]
        public async Task<IActionResult> GetByRole(string roleName)
        {
            var result = await _accountService.GetAccountsByRoleAsync(roleName);
            return Ok(result);
        }
    }
}
