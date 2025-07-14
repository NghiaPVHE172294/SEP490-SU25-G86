using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Cv
    {
        public Cv()
        {
            CvparsedData = new HashSet<CvparsedDatum>();
            Cvsubmissions = new HashSet<Cvsubmission>();
        }

        public int CvId { get; set; }
        public int? CandidateId { get; set; }
        public string FileUrl { get; set; } = null!;
        public string? Notes { get; set; }
        public bool? SaveStatus { get; set; }
        public bool IsDelete { get; set; }
        public int UploadByUserId { get; set; }
        public DateTime? UploadDate { get; set; }
        public string? Cvname { get; set; }

        public virtual User UploadByUser { get; set; } = null!;
        public virtual ICollection<CvparsedDatum> CvparsedData { get; set; }
        public virtual ICollection<Cvsubmission> Cvsubmissions { get; set; }
    }
}
