using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.Services.GeminiCvJobMatchingService;
using System.Threading.Tasks;
using vn.edu.fpt.DTOs.GeminiDTO;

namespace SEP490_SU25_G86_API.Controllers.AIController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AIController : ControllerBase
    {
        private readonly IGeminiCvJobMatchingService _geminiService;
        public AIController(IGeminiCvJobMatchingService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpPost("CompareCvWithJobCriteria")]
        public async Task<IActionResult> CompareCvWithJobCriteria([FromBody] CompareCvWithJobCriteriaRequest request)
        {
            if (request == null || request.CvParsedDataId <= 0 || request.JobCriteriaId <= 0)
                return BadRequest("Thiếu thông tin đầu vào");

            var result = await _geminiService.CompareCvWithJobCriteriaAsync(request.CvParsedDataId, request.JobCriteriaId);
            if (result == null)
                return StatusCode(500, "Không thể so sánh hoặc lưu kết quả");
            // Map sang DTO để tránh vòng lặp khi serialize
            var dto = MatchedCvandJobPostMapper.ToDto(result);
            return Ok(dto);
        }
    }

    public class CompareCvWithJobCriteriaRequest
    {
        public int CvParsedDataId { get; set; }
        public int JobCriteriaId { get; set; }
    }
}
