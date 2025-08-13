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
            if (request.PdfFile == null || request.DocFile == null || request.PreviewImage == null)
                return BadRequest("Cần upload đủ cả file PDF, DOCX và ảnh minh họa.");

            // Kiểm tra định dạng và dung lượng PDF
            if (request.PdfFile.ContentType != "application/pdf" || request.PdfFile.Length > 5 * 1024 * 1024)
                return BadRequest("File PDF phải đúng định dạng (PDF) và dưới 5MB!");

            // Kiểm tra định dạng và dung lượng DOCX
            if ((request.DocFile.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document" &&
                 request.DocFile.ContentType != "application/msword") || request.DocFile.Length > 5 * 1024 * 1024)
                return BadRequest("File DOCX phải đúng định dạng (DOCX/DOC) và dưới 5MB!");

            // Kiểm tra định dạng và dung lượng ảnh minh họa (JPEG/PNG, < 5MB)
            if ((request.PreviewImage.ContentType != "image/png" && request.PreviewImage.ContentType != "image/jpeg") || request.PreviewImage.Length > 5 * 1024 * 1024)
                return BadRequest("Ảnh minh họa phải là PNG hoặc JPEG và dưới 5MB!");

            var (pdfUrl, docUrl, imgUrl) = await _uploadService.UploadCvTemplateAsync(request.PdfFile, request.DocFile, request.PreviewImage);
            // Lưu vào DB (CvTemplate)
            using (var db = new SEP490_SU25_G86_API.Models.SEP490_G86_CvMatchContext())
            {
                var template = new SEP490_SU25_G86_API.Models.CvTemplate
                {
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
    }
}
