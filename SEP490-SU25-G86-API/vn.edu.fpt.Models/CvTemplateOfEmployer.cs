using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class CvTemplateOfEmployer
    {
        public CvTemplateOfEmployer()
        {
            CvTemplateForJobposts = new HashSet<CvTemplateForJobpost>();
        }

        public int CvtemplateOfEmployerId { get; set; }
        public int EmployerId { get; set; }
        public string? CvTemplateName { get; set; }
        public string? DocFileUrl { get; set; }
        public string? PdfFileUrl { get; set; }
        public DateTime? UploadDate { get; set; }
        public string? Notes { get; set; }
        public bool IsDelete { get; set; }

        public virtual User Employer { get; set; } = null!;
        public virtual ICollection<CvTemplateForJobpost> CvTemplateForJobposts { get; set; }
    }
}
