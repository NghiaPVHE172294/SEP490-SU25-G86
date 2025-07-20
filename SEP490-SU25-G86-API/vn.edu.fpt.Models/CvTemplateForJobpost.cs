using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class CvTemplateForJobpost
    {
        public int CvTemplateForJobpostId { get; set; }
        public int CvtemplateOfEmployerId { get; set; }
        public int JobPostId { get; set; }
        public bool? IsDisplay { get; set; }

        public virtual CvTemplateOfEmployer CvtemplateOfEmployer { get; set; } = null!;
        public virtual JobPost JobPost { get; set; } = null!;
    }
}
