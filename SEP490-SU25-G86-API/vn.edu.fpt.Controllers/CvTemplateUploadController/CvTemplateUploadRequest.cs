using Microsoft.AspNetCore.Http;

namespace vn.edu.fpt.Controllers.CvTemplateUpload
{
    public class CvTemplateUploadRequest
    {
        public IFormFile PdfFile { get; set; }
        public IFormFile? DocFile { get; set; } // Cho phép null
        public IFormFile PreviewImage { get; set; } // Ảnh minh họa
        public int IndustryId { get; set; }
        public int PositionId { get; set; }
        public string CvTemplateName { get; set; }
        public string? Notes { get; set; }
    }
}
