using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using vn.edu.fpt.Services.CvTemplateUpload;

namespace vn.edu.fpt.Controllers.CvTemplateUpload
{
    [ApiController]
    [Route("api/admin/cv-templates")] // Chuẩn RESTful cho quản lý template
    public class CvTemplateUploadController : ControllerBase
    {
        private readonly ICvTemplateUploadService _uploadService;
        public CvTemplateUploadController(ICvTemplateUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadCvTemplate([FromForm] CvTemplateUploadRequest request)
        {
            if (request.PdfFile == null || request.PreviewImage == null)
                return BadRequest("Cần upload đủ file PDF và ảnh minh họa.");

            // Kiểm tra định dạng và dung lượng PDF
            if (request.PdfFile.ContentType != "application/pdf" || request.PdfFile.Length > 5 * 1024 * 1024)
                return BadRequest("File PDF phải đúng định dạng (PDF) và dưới 5MB!");

            // Kiểm tra định dạng và dung lượng DOC/DOCX (nếu có)
            if (request.DocFile != null)
            {
                if ((!request.DocFile.FileName.EndsWith(".doc", StringComparison.OrdinalIgnoreCase) && !request.DocFile.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)) || request.DocFile.Length > 5 * 1024 * 1024)
                    return BadRequest("File DOC/DOCX phải đúng định dạng (.doc/.docx) và dưới 5MB!");
                if ((request.DocFile.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" &&
                     request.DocFile.ContentType != "application/msword"))
                    return BadRequest("File DOCX phải đúng định dạng (DOCX/DOC)!");
            }

            // Kiểm tra định dạng và dung lượng ảnh minh họa (JPEG/PNG, < 5MB)
            if ((request.PreviewImage.ContentType != "image/png" && request.PreviewImage.ContentType != "image/jpeg") || request.PreviewImage.Length > 5 * 1024 * 1024)
                return BadRequest("Ảnh minh họa phải là PNG hoặc JPEG và dưới 5MB!");

            string pdfUrl, docUrl = null, imgUrl;
if (request.DocFile != null)
{
    (pdfUrl, docUrl, imgUrl) = await _uploadService.UploadCvTemplateAsync(request.PdfFile, request.DocFile, request.PreviewImage);
}
else
{
    (pdfUrl, _, imgUrl) = await _uploadService.UploadCvTemplateAsync(request.PdfFile, null, request.PreviewImage);
}
            // Lưu vào DB (CvTemplate)
            using (var db = new SEP490_SU25_G86_API.Models.SEP490_G86_CvMatchContext())
            {
                // Lấy AccountId từ claim
var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
if (accountIdClaim == null)
    return Unauthorized("Không tìm thấy thông tin tài khoản đăng nhập.");
int accountId = int.Parse(accountIdClaim.Value);
// Lấy UserId tương ứng
var adminUser = db.Users.FirstOrDefault(u => u.AccountId == accountId);
if (adminUser == null)
    return Unauthorized("Không tìm thấy user tương ứng với tài khoản này.");
int adminUserId = adminUser.UserId;

var template = new SEP490_SU25_G86_API.Models.CvTemplate
{
    AdminId = adminUserId,
    PdfFileUrl = pdfUrl,
    DocFileUrl = docUrl,
    ImgTemplate = imgUrl,
    IndustryId = request.IndustryId,
    PositionId = request.PositionId,
    CvTemplateName = request.CvTemplateName,
    Notes = request.Notes,
    UploadDate = DateTime.Now,
    IsDelete = false
};
                db.CvTemplates.Add(template);
                await db.SaveChangesAsync();
                return Ok(new {
                    id = template.CvTemplateId,
                    pdfUrl,
                    docUrl,
                    imgUrl,
                    industryId = request.IndustryId,
                    positionId = request.PositionId,
                    cvTemplateName = request.CvTemplateName,
                    notes = request.Notes,
                    uploadDate = template.UploadDate
                });
            }
        }
        [HttpGet]
public IActionResult GetAllTemplates()
{
    using (var db = new SEP490_SU25_G86_API.Models.SEP490_G86_CvMatchContext())
    {
        // Lấy AccountId từ claim
        var accountIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (accountIdClaim == null)
            return Unauthorized("Không tìm thấy thông tin tài khoản đăng nhập.");
        int accountId = int.Parse(accountIdClaim.Value);
        // Lấy UserId tương ứng
        var adminUser = db.Users.FirstOrDefault(u => u.AccountId == accountId);
        if (adminUser == null)
            return Unauthorized("Không tìm thấy user tương ứng với tài khoản này.");
        int adminUserId = adminUser.UserId;

        var templates = db.CvTemplates
            .Where(t => t.IsDelete != true && t.AdminId == adminUserId)
            .OrderByDescending(t => t.UploadDate)
            .Select(t => new {
                t.CvTemplateId,
                t.CvTemplateName,
                t.PdfFileUrl,
                t.DocFileUrl,
                t.ImgTemplate,
                t.Notes,
                t.UploadDate,
                t.IndustryId,
                t.PositionId
            })
            .ToList()
            .Select(t => new {
                t.CvTemplateId,
                t.CvTemplateName,
                t.PdfFileUrl,
                t.DocFileUrl,
                t.ImgTemplate,
                t.Notes,
                UploadDate = t.UploadDate != null ? t.UploadDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                t.IndustryId,
                t.PositionId
            })
            .ToList();
        return Ok(templates);
    }
}

        [HttpDelete("{id}")]
        public IActionResult DeleteTemplate(int id)
        {
            using (var db = new SEP490_SU25_G86_API.Models.SEP490_G86_CvMatchContext())
            {
                var template = db.CvTemplates.FirstOrDefault(t => t.CvTemplateId == id && t.IsDelete != true);
                if (template == null) return NotFound();
                template.IsDelete = true;
                db.SaveChanges();
                return Ok();
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTemplate(int id, [FromBody] SEP490_SU25_G86_API.DTOs.CvTemplateDTO.UpdateCvTemplateRequest request)
        {
            using (var db = new SEP490_SU25_G86_API.Models.SEP490_G86_CvMatchContext())
            {
                var template = db.CvTemplates.FirstOrDefault(t => t.CvTemplateId == id && t.IsDelete != true);
                if (template == null) return NotFound();
                if (!string.IsNullOrWhiteSpace(request.CvTemplateName))
                    template.CvTemplateName = request.CvTemplateName;
                if (request.Notes != null)
                    template.Notes = request.Notes;
                db.SaveChanges();
                return Ok();
            }
        }
    }
}
