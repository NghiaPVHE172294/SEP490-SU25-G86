using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobCriterionDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobCriterionService;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.JobCriterionController
{
    /// <summary>
    /// Controller xử lý các API liên quan đến tiêu chí công việc (Job Criteria) của người dùng.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class JobCriterionController : ControllerBase
    {
        private readonly IJobCriterionService _service;
        private readonly SEP490_G86_CvMatchContext _context;

        public JobCriterionController(IJobCriterionService service, SEP490_G86_CvMatchContext context)
        {
            _service = service;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách tiêu chí công việc của user hiện tại (đang đăng nhập).
        /// </summary>
        /// <returns>Danh sách các tiêu chí công việc.</returns>
        [HttpGet("my")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Thành công
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Không xác thực
        public async Task<IActionResult> GetMyJobCriteria()
        {
            var accountIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(accountIdStr, out int accountId))
                return Unauthorized("Không xác định được tài khoản.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);

            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");

            var result = await _service.GetJobCriteriaByUserIdAsync(user.UserId);

            return Ok(result);
        }

        /// <summary>
        /// Thêm một tiêu chí công việc mới cho user hiện tại.
        /// </summary>
        /// <param name="dto">Thông tin tiêu chí cần thêm.</param>
        /// <returns>Kết quả thêm tiêu chí.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)] // Thành công
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Dữ liệu sai
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Không xác thực
        public async Task<IActionResult> AddJobCriterion([FromBody] AddJobCriterionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); // Kiểm tra dữ liệu đầu vào

            var accountIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(accountIdStr, out int accountId))
                return Unauthorized("Không xác định được tài khoản.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);

            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");

            var result = await _service.AddJobCriterionAsync(dto, user.UserId);

            return Ok(result);
        }

        /// <summary>
        /// Cập nhật một tiêu chí công việc cho user hiện tại.
        /// </summary>
        /// <param name="dto">Thông tin tiêu chí cần cập nhật.</param>
        /// <returns>Kết quả cập nhật tiêu chí.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateJobCriterion([FromBody] UpdateJobCriterionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var accountIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(accountIdStr, out int accountId))
                return Unauthorized("Không xác định được tài khoản.");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");
            var result = await _service.UpdateJobCriterionAsync(dto, user.UserId);
            return Ok(result);
        }
        /// <summary>
        /// Xóa mềm một tiêu chí công việc (set IsDelete = true).
        /// </summary>
        /// <param name="id">ID của JobCriterion.</param>
        /// <returns>Kết quả của thao tác xóa.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Thành công
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Không tìm thấy tiêu chí công việc
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Không xác thực
        public async Task<IActionResult> SoftDeleteJobCriterion(int id)
        {
            var accountIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(accountIdStr, out int accountId))
                return Unauthorized("Không xác định được tài khoản.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");

            var result = await _service.SoftDeleteJobCriterionAsync(id, user.UserId);
            if (!result)
                return NotFound("JobCriterion không tồn tại hoặc bạn không có quyền xóa.");

            return NoContent(); // Thành công, trả về status 204
        }

        /// <summary>
        /// Khôi phục một tiêu chí công việc (set IsDelete = false).
        /// </summary>
        /// <param name="id">ID của JobCriterion.</param>
        /// <returns>Kết quả của thao tác khôi phục.</returns>
        [HttpPost("{id}/restore")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Thành công
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Không tìm thấy tiêu chí công việc
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Không xác thực
        public async Task<IActionResult> RestoreJobCriterion(int id)
        {
            var accountIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdStr) || !int.TryParse(accountIdStr, out int accountId))
                return Unauthorized("Không xác định được tài khoản.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");

            var result = await _service.RestoreJobCriterionAsync(id, user.UserId);
            if (!result)
                return NotFound("JobCriterion không tồn tại hoặc bạn không có quyền khôi phục.");

            return NoContent(); // Thành công, trả về status 204
        }
    }
}
