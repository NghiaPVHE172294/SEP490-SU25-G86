using Microsoft.AspNetCore.Http;

namespace SEP490_SU25_G86_API.DTOs.CvTemplateDTO
{
    public class CvTemplateUploadRequest
    {
        public IFormFile PdfFile { get; set; }
        public IFormFile? DocFile { get; set; } // Không Required, cho phép null
        public string CvTemplateName { get; set; }
        public string? Notes { get; set; }
    }
}
