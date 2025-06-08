using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Cvsubmission
    {
        public int SubmissionId { get; set; }
        public int? CvId { get; set; }
        public int? JobPostId { get; set; }
        public int? SubmittedByUserId { get; set; }
        public string? SourceType { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public int? LabelId { get; set; }
        public string? LabelSource { get; set; }
        public bool? IsShortlisted { get; set; }
        public string? RecruiterNote { get; set; }

        public virtual Cv? Cv { get; set; }
        public virtual JobPost? JobPost { get; set; }
        public virtual Cvlabel? Label { get; set; }
        public virtual User? SubmittedByUser { get; set; }
    }
}
