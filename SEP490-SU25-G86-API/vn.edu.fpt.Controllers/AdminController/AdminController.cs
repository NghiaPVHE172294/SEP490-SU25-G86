using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.Repositories.AdminDashboardRepository;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.AdminDashboardServices;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.AdminController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        [HttpGet("Dashboard/JobPostPerMonth")]
        public IActionResult GetMonthlyJobPostStats()
        {
            var stats = _adminDashboardService.GetMonthlyJobPostStatistics();
            return Ok(stats);
        }
    }
} 