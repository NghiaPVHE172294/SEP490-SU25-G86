﻿using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class CvTemplate
    {
        public int CvTemplateId { get; set; }
        public string PdfFileUrl { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime? UploadDate { get; set; }
        public bool? IsDelete { get; set; }
        public int IndustryId { get; set; }
        public string? CvTemplateName { get; set; }
        public string DocFileUrl { get; set; } = null!;
        public int PositionId { get; set; }

        public virtual Industry Industry { get; set; } = null!;
        public virtual JobPosition Position { get; set; } = null!;
    }
}
