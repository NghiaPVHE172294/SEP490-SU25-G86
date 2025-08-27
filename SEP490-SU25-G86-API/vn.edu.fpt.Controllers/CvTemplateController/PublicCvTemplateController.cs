using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SEP490_SU25_G86_API.Models;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.CvTemplateController
{
    
    [ApiController]
    [AllowAnonymous]
    [Route("api/public/cv-templates")]
    public class PublicCvTemplateController : ControllerBase
    {
        [HttpGet]
public async Task<IActionResult> GetAll(
    [FromServices] SEP490_G86_CvMatchContext dbContext,
    [FromQuery] int? industryId,
    [FromQuery] int? positionId,
    [FromQuery] string? search,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 3)
{
    var query = dbContext.CvTemplates
        .Where(t => t.IsDelete == false || t.IsDelete == null);

    if (industryId.HasValue)
        query = query.Where(t => t.IndustryId == industryId);
    if (positionId.HasValue)
        query = query.Where(t => t.PositionId == positionId);
    if (!string.IsNullOrWhiteSpace(search))
        query = query.Where(t => t.CvTemplateName.Contains(search));

    var totalCount = await query.CountAsync();

    var templates = await query
        .OrderByDescending(t => t.UploadDate)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(t => new
        {
            t.CvTemplateId,
            t.CvTemplateName,
            t.PdfFileUrl,
            t.DocFileUrl,
            t.ImgTemplate,
            t.UploadDate,
            t.Notes,
            t.IndustryId,
            t.PositionId,
            IndustryName = t.Industry != null ? t.Industry.IndustryName : null,
            PositionName = t.Position != null ? t.Position.PostitionName : null
        })
        .ToListAsync();

    return Ok(new { data = templates, totalCount });
}
    }
}
