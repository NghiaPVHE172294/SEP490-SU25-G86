using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SEP490_SU25_G86_API.Models;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.ParseCvDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.ParseCvService;
using System.Net.Http.Headers;
using System.Text;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.ParseCVController
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class ParseCvController : ControllerBase
    {
        private readonly IParseCvService _service;
        public ParseCvController(IParseCvService service)
        {
            _service = service;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCv([FromForm] UploadCvRequest request)
        {
            try
            {
                var parsed = await _service.ParseAndSaveAsync(request.File);

                // Map sang DTO
                var dto = new CvParsedDto
                {
                    FullName = parsed.FullName,
                    Email = parsed.Email,
                    Phone = parsed.Phone,
                    YearsOfExperience = (float?)parsed.YearsOfExperience,
                    Skills = parsed.Skills,
                    EducationLevel = parsed.EducationLevel,
                    JobTitles = parsed.JobTitles,
                    Languages = parsed.Languages,
                    Certifications = parsed.Certifications,
                    Address = parsed.Address,
                    Summary = parsed.Summary,
                    WorkHistory = parsed.WorkHistory,
                    Projects = parsed.Projects
                };

                return Ok(dto);
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("rate limit"))
            {
                // Lỗi rate limit từ OpenAI
                return StatusCode(429, new { error = "OpenAI rate limit exceeded. Vui lòng thử lại sau." });
            }
            catch (InvalidOperationException ex)
            {
                // Lỗi khi không thể parse JSON từ GPT hoặc lỗi nghiệp vụ khác
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }

    }
}
