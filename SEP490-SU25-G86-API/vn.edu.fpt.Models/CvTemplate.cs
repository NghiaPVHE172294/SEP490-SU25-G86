using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class CvTemplate
    {
        public int CvTemplateId { get; set; }
        public int AdminId { get; set; }
        public string PdfFileUrl { get; set; } = null!;
        public string? ImgTemplate { get; set; }
        public string? Notes { get; set; }
        public DateTime? UploadDate { get; set; }
        public int IndustryId { get; set; }
        public string? CvTemplateName { get; set; }
        public string? DocFileUrl { get; set; } // Cho phép null
        public int PositionId { get; set; }
        public bool? IsDelete { get; set; }

        public virtual User Admin { get; set; } = null!;
        public virtual Industry Industry { get; set; } = null!;
        public virtual JobPosition Position { get; set; } = null!;
    }
}
