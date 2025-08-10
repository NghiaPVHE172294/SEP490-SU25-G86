using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SEP490_SU25_G86_API.DTOs.CvTemplateDTO;
using SEP490_SU25_G86_API.Services.CvTemplateService;
using SEP490_SU25_G86_API.Models;
using SEP490_G86_CvMatchContext = SEP490_SU25_G86_API.Models.SEP490_G86_CvMatchContext;

namespace SEP490_SU25_G86_API.vn.edu.fpt.Controllers.CvTemplateController
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "EMPLOYER")]
    public class EmployerController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            return Ok(new { message = "Chào mừng Employer! Đây là dashboard dành cho Employer." });
        }

        [HttpGet("cv-templates")]
        public async Task<IActionResult> GetCvTemplates([FromServices] SEP490_G86_CvMatchContext dbContext)
        {
            // Lấy employerId từ claim
            var employerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId" || c.Type.EndsWith("nameidentifier"));
            if (employerIdClaim == null) return Unauthorized();
            int employerId = int.Parse(employerIdClaim.Value);

            var templates = await dbContext.CvTemplateOfEmployers
                .Where(t => t.EmployerId == employerId && !t.IsDelete)
                .OrderByDescending(t => t.UploadDate)
                .Select(t => new
                {
                    t.CvtemplateOfEmployerId,
                    t.CvTemplateName,
                    t.PdfFileUrl,
                    t.DocFileUrl,
                    t.UploadDate,
                    t.Notes
                })
                .ToListAsync();

            return Ok(templates);
        }
        [HttpPost("cv-templates/upload")]
        public async Task<IActionResult> UploadCvTemplate(
            [FromServices] IFirebaseStorageService firebaseService,
            [FromServices] SEP490_G86_CvMatchContext dbContext,
            [FromForm] CvTemplateUploadRequest request)
        {
            if (request.PdfFile == null || request.DocFile == null)
                return BadRequest("Cần chọn đủ file PDF và DOC/DOCX");
            if (!request.PdfFile.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ||
                !(request.DocFile.FileName.EndsWith(".doc", StringComparison.OrdinalIgnoreCase) || request.DocFile.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)))
                return BadRequest("Sai định dạng file");
            // Lấy employerId từ claim
            var employerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId" || c.Type.EndsWith("nameidentifier"));
            if (employerIdClaim == null) return Unauthorized();
            int employerId = int.Parse(employerIdClaim.Value);
            // Upload lên Firebase
            string pdfUrl = await firebaseService.UploadFileAsync(request.PdfFile, "cvtemplates");
            string docUrl = await firebaseService.UploadFileAsync(request.DocFile, "cvtemplates");
            // Lưu DB
            var entity = new Models.CvTemplateOfEmployer
            {
                EmployerId = employerId,
                CvTemplateName = request.CvTemplateName,
                PdfFileUrl = pdfUrl,
                DocFileUrl = docUrl,
                UploadDate = DateTime.UtcNow,
                Notes = request.Notes,
                IsDelete = false
            };
            dbContext.CvTemplateOfEmployers.Add(entity);
            await dbContext.SaveChangesAsync();
            return Ok(new { entity.CvtemplateOfEmployerId, entity.CvTemplateName, entity.PdfFileUrl, entity.DocFileUrl });
        }

        [HttpDelete("cv-templates/{id}")]
        public async Task<IActionResult> DeleteCvTemplate(int id, [FromServices] SEP490_G86_CvMatchContext dbContext)
        {
            // Lấy employerId từ claim
            var employerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId" || c.Type.EndsWith("nameidentifier"));
            if (employerIdClaim == null) return Unauthorized();
            int employerId = int.Parse(employerIdClaim.Value);

            var template = await dbContext.CvTemplateOfEmployers
                .FirstOrDefaultAsync(t => t.CvtemplateOfEmployerId == id && t.EmployerId == employerId && !t.IsDelete);

            if (template == null)
                return NotFound("Không tìm thấy CV template hoặc bạn không có quyền xóa");

            // Soft delete
            template.IsDelete = true;
            await dbContext.SaveChangesAsync();

            return Ok(new { message = "Xóa CV template thành công" });
        }

        [HttpPut("cv-templates/{id}")]
        public async Task<IActionResult> UpdateCvTemplate(int id, [FromBody] UpdateCvTemplateRequest request, [FromServices] SEP490_G86_CvMatchContext dbContext)
        {
            // Lấy employerId từ claim
            var employerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId" || c.Type.EndsWith("nameidentifier"));
            if (employerIdClaim == null) return Unauthorized();
            int employerId = int.Parse(employerIdClaim.Value);

            var template = await dbContext.CvTemplateOfEmployers
                .FirstOrDefaultAsync(t => t.CvtemplateOfEmployerId == id && t.EmployerId == employerId && !t.IsDelete);

            if (template == null)
                return NotFound("Không tìm thấy CV template hoặc bạn không có quyền sửa");

            // Cập nhật thông tin
            if (!string.IsNullOrWhiteSpace(request.CvTemplateName))
                template.CvTemplateName = request.CvTemplateName;
            
            if (request.Notes != null)
                template.Notes = request.Notes;

            await dbContext.SaveChangesAsync();

            return Ok(new { 
                template.CvtemplateOfEmployerId, 
                template.CvTemplateName, 
                template.Notes,
                message = "Cập nhật CV template thành công" 
            });
        }
    }
}