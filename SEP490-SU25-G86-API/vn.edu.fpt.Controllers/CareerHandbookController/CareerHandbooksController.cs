using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.CareerHandbookDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CareerHandbookService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CareerHandbooksController : ControllerBase
    {
        private readonly ICareerHandbookService _service;

        public CareerHandbooksController(ICareerHandbookService service)
        {
            _service = service;
        }

        // ADMIN: List all
        [HttpGet("admin")]
        [Authorize]
        public async Task<IActionResult> GetAllForAdmin()
        {
            var data = await _service.GetAllForAdminAsync();
            return Ok(data);
        }

        // USER: List published
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPublished()
        {
            var data = await _service.GetAllPublishedAsync();
            return Ok(data);
        }

        // USER: View by slug
        [HttpGet("view/{slug}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var handbook = await _service.GetBySlugAsync(slug);
            if (handbook == null) return NotFound();
            return Ok(handbook);
        }

        // ADMIN: Get by ID
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var handbook = await _service.GetByIdAsync(id);
            if (handbook == null) return NotFound();
            return Ok(handbook);
        }

        // ADMIN: Create
        [HttpPost]
        [Authorize]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Create([FromForm] CareerHandbookCreateDTO dto)
        {
            try
            {
                await _service.CreateAsync(dto);
                return Ok(new { message = "Tạo bài viết thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        // ADMIN: Update
        [HttpPut("{id}")]
        [Authorize]
        [RequestSizeLimit(10 * 1024 * 1024)]
        public async Task<IActionResult> Update(int id, [FromForm] CareerHandbookUpdateDTO dto)
        {
            try
            {
                var success = await _service.UpdateAsync(id, dto);
                if (!success) return NotFound();
                return Ok(new { message = "Cập nhật bài viết thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // ADMIN: Soft delete
        [HttpDelete("admin/{id}")]
        [Authorize]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await _service.SoftDeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
