using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.JobPostDTO;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.JobController
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostsController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;
        private readonly SEP490_G86_CvMatchContext _context;

        public JobPostsController(IJobPostService jobPostService, SEP490_G86_CvMatchContext context)
        {
            _jobPostService = jobPostService;
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách bài tuyển dụng hiển thị trên trang chủ (có phân trang và lọc theo vùng).
        /// </summary>
        /// <param name="page">Số trang (mặc định là 1)</param>
        /// <param name="pageSize">Số lượng bài tuyển dụng trên mỗi trang (mặc định là 9)</param>
        /// <param name="region">Vùng lọc (nếu có)</param>
        /// <returns>Danh sách bài tuyển dụng và tổng số bài</returns>
        /// <response code="200">Trả về danh sách bài tuyển dụng</response>
        /// <response code="500">Lỗi server trong quá trình xử lý</response>
        [HttpGet("homepage")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHomeJobs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 9,
        [FromQuery] string? region = null,
        [FromQuery] int? salaryRangeId = null,
        [FromQuery] int? experienceLevelId = null)
        {
            int? candidateId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("CANDIDATE"))
            {
                var accountIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(accountIdStr, out int accountId))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
                    if (user != null)
                        candidateId = user.UserId;
                }
            }

            var jobs = await _jobPostService.GetPagedJobPostsAsync(
                page, pageSize, region, salaryRangeId, experienceLevelId, candidateId);

            return Ok(new
            {
                TotalItems = jobs.Item2,
                Jobs = jobs.Item1
            });
        }

        /// <summary>
        /// Lấy tất cả bài tuyển dụng trong hệ thống.
        /// </summary>
        /// <returns>Danh sách tất cả bài tuyển dụng</returns>
        /// <response code="200">Thành công, trả về danh sách bài tuyển dụng</response>
        /// <response code="500">Lỗi server khi truy vấn dữ liệu</response>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetAllJobPosts()
        {
            var result = await _jobPostService.GetAllJobPostsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách bài tuyển dụng theo employer (nhà tuyển dụng).
        /// </summary>
        /// <returns>Danh sách bài tuyển dụng theo employer</returns>
        /// <response code="200">Thành công, trả về danh sách</response>
        /// <response code="404">Không tìm thấy employer hoặc không có bài nào</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("employer")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<JobPostListDTO>>> GetByEmployerId()
        {
            var accountId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
            {
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");
            }
            var result = await _jobPostService.GetByEmployerIdAsync(user.UserId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy chi tiết một bài tuyển dụng theo id
        /// </summary>
        /// <param name="id">Id của JobPost</param>
        /// <returns>Chi tiết JobPost</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobPostDetail(int id)
        {
            try
            {
                var detail = await _jobPostService.GetJobPostDetailByIdAsync(id);
                if (detail == null) return NotFound();
                return Ok(detail);
            }
            catch (Exception ex)
            {
                // Để middleware xử lý exception chung
                throw;
            }
        }
        [HttpGet("viewall")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPagedJobPosts(
                int page = 1,
                int pageSize = 10,
                int? provinceId = null,
                int? industryId = null,
                [FromQuery] List<int>? employmentTypeIds = null,
                [FromQuery] List<int>? experienceLevelIds = null,
                int? jobLevelId = null,
                int? minSalary = null,
                int? maxSalary = null,
                [FromQuery] List<int>? datePostedRanges = null,
                [FromQuery] string? keyword = null)
        {
            int? candidateId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("CANDIDATE"))
            {
                var accountIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(accountIdStr, out int accountId))
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
                    if (user != null)
                        candidateId = user.UserId;
                }
            }
            var (posts, totalItems) = await _jobPostService.GetFilteredJobPostsAsync(
                page, pageSize, provinceId, industryId, employmentTypeIds, experienceLevelIds, jobLevelId, minSalary, maxSalary, datePostedRanges, keyword, candidateId
            );
            return Ok(new { posts, totalItems });
        }

        /// <summary>
        /// Tạo mới một bài tuyển dụng
        /// </summary>
        /// <param name="dto">Thông tin job post cần tạo</param>
        /// <returns>Chi tiết job post vừa tạo</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddJobPost([FromBody] AddJobPostDTO dto)
        {
            var accountId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");
            var result = await _jobPostService.AddJobPostAsync(dto, user.UserId);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật một bài tuyển dụng
        /// </summary>
        /// <param name="dto">Thông tin job post cần cập nhật</param>
        /// <returns>Chi tiết job post vừa cập nhật</returns>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateJobPost([FromBody] UpdateJobPostDTO dto)
        {
            var accountId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");
            var result = await _jobPostService.UpdateJobPostAsync(dto, user.UserId);
            return Ok(result);
        }

        [HttpGet("{id}/jobposts")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobPostsByCompanyId(int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var (posts, totalItems) = await _jobPostService.GetJobPostsByCompanyIdAsync(id, page, pageSize);
            return Ok(new { posts, totalItems });
        }

        /// <summary>
        /// Xóa mềm JobPost (IsDelete = true).
        /// - ADMIN: xóa được tất cả
        /// - EMPLOYER: chỉ xóa bài của mình
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteJobPost(int id)
        {
            var accountIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(accountIdStr, out int accountId))
                return Unauthorized("Không xác thực được người dùng.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");

            bool isAdmin = User.IsInRole("ADMIN");

            var ok = await _jobPostService.DeleteJobPostAsync(id, user.UserId, isAdmin);
            if (!ok) return NotFound("JobPost không tồn tại hoặc bạn không có quyền xóa.");

            return NoContent();
        }

        /// <summary>
        /// Khôi phục JobPost đã xóa mềm (IsDelete = false).
        /// - ADMIN: khôi phục được tất cả
        /// - EMPLOYER: chỉ khôi phục bài của mình
        /// </summary>
        [HttpPost("{id}/restore")]
        [Authorize]
        public async Task<IActionResult> RestoreJobPost(int id)
        {
            var accountIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(accountIdStr, out int accountId))
                return Unauthorized("Không xác thực được người dùng.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.AccountId == accountId);
            if (user == null)
                return Unauthorized("Không tìm thấy người dùng tương ứng với tài khoản.");

            bool isAdmin = User.IsInRole("ADMIN");

            var ok = await _jobPostService.RestoreJobPostAsync(id, user.UserId, isAdmin);
            if (!ok) return NotFound("JobPost không tồn tại hoặc bạn không có quyền khôi phục.");

            return NoContent();
        }
        /// <summary>
        /// Lấy danh sách việc làm liên quan theo IndustryId.
        /// </summary>
        /// <param name="industryId">Id ngành</param>
        /// <param name="take">Số lượng tối đa (mặc định 5)</param>
        /// <param name="excludeJobPostId">Loại trừ JobPost hiện tại (optional)</param>
        [HttpGet("related-jobs/{industryId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<RelatedJobItemDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRelatedJobs(
            [FromRoute] int industryId,
            [FromQuery] int take = 5,
            [FromQuery] int? excludeJobPostId = null,
            CancellationToken ct = default)
        {
            if (industryId <= 0) return BadRequest("industryId invalid.");

            var items = await _jobPostService.GetRelatedJobsAsync(industryId, take, excludeJobPostId, ct);
            // Trả 200 kèm mảng rỗng nếu không có gì – frontend xử lý hiển thị.
            return Ok(items);
        }
    }
}
