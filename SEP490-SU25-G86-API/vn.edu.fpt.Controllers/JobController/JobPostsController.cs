using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SEP490_SU25_G86_API.vn.edu.fpt.DTO.JobPostDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.JobPostService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.JobController
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostsController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;

        public JobPostsController(IJobPostService jobPostService)
        {
            _jobPostService = jobPostService;
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
            [FromQuery] string? region = null)
        {
            var jobs = await _jobPostService.GetPagedJobPostsAsync(page, pageSize, region);

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
        /// <param name="employerId">ID của employer</param>
        /// <returns>Danh sách bài tuyển dụng theo employer</returns>
        /// <response code="200">Thành công, trả về danh sách</response>
        /// <response code="404">Không tìm thấy employer hoặc không có bài nào</response>
        /// <response code="500">Lỗi server</response>
        [HttpGet("employer/{employerId}")]
        public async Task<ActionResult<IEnumerable<JobPostDTO>>> GetByEmployerId(int employerId)
        {
            var result = await _jobPostService.GetByEmployerIdAsync(employerId);
            return Ok(result);
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
            string? keyword = null)
        {
            var (posts, totalItems) = await _jobPostService.GetFilteredJobPostsAsync(
                page, pageSize, provinceId, industryId, employmentTypeIds, experienceLevelIds, jobLevelId, minSalary, maxSalary, datePostedRanges, keyword
            );
            return Ok(new { posts, totalItems });
        }
    }
}
