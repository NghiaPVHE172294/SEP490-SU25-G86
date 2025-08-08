using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.HandbookCategoryDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.HandbookCategoryService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HandbookCategoriesController : ControllerBase
    {
        private readonly IHandbookCategoryService _service;

        public HandbookCategoriesController(IHandbookCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var data = await _service.GetAllAsync();
            return Ok(data);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] HandbookCategoryCreateDTO dto)
        {
            try
            {
                await _service.CreateAsync(dto);
                return Ok(new { message = "Tạo danh mục thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] HandbookCategoryUpdateDTO dto)
        {
            try
            {
                var success = await _service.UpdateAsync(id, dto);
                if (!success) return NotFound();
                return Ok(new { message = "Cập nhật danh mục thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _service.DeleteAsync(id);
                if (!success) return NotFound();
                return Ok(new { message = "Xóa danh mục thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
