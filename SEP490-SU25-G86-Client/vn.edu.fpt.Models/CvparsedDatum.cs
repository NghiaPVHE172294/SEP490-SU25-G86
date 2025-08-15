using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class CvparsedDatum
    {
        public CvparsedDatum()
        {
            MatchedCvandJobPosts = new HashSet<MatchedCvandJobPost>();
        }

        public int CvparsedDataId { get; set; }
        public int CvId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public double? YearsOfExperience { get; set; }
        public string? Skills { get; set; }
        public string? EducationLevel { get; set; }
        public string? JobTitles { get; set; }
        public string? Languages { get; set; }
        public string? Certifications { get; set; }
        public DateTime ParsedAt { get; set; }
        public string? Address { get; set; }
        public string? Summary { get; set; }
        public string? WorkHistory { get; set; }
        public string? Projects { get; set; }
        public bool IsDelete { get; set; }

        public virtual Cv Cv { get; set; } = null!;
        public virtual ICollection<MatchedCvandJobPost> MatchedCvandJobPosts { get; set; }
    }
}
