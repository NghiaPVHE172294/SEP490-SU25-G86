using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SEP490_SU25_G86_API.vn.edu.fpt.DTOs.ParseCvDTO;
using SEP490_SU25_G86_API.vn.edu.fpt.Services.CVParsedDataService;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.CvParseController
{
    [ApiController]
    [Route("api/cv")]
    [AllowAnonymous]
    public class CvParseController : ControllerBase
    {
        private readonly ICvParsingService _service;
        public CvParseController(ICvParsingService service) => _service = service;

        // POST /api/cv/parse
        [HttpPost("parse")]
        [RequestSizeLimit(200_000_000)] // ~200MB
        public async Task<ActionResult<ParseCvResult>> Parse([FromForm] ParseCvRequest req, IFormFile file, CancellationToken ct)
        {
            if (file == null || file.Length == 0) return BadRequest("Thiếu file.");
            if (req.CVId <= 0) return BadRequest("CVId không hợp lệ.");

            var saved = await _service.ParseAndSaveAsync(req.CVId, file, req.Prompt, ct);
            return Ok(new ParseCvResult { CVParsedDataId = saved.CvparsedDataId, CVId = saved.CvId });
        }
    }
}
