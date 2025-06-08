using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class CvparsedDatum
    {
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
        public int MatchedJobCriteriaId { get; set; }
        public double? MatchingScore { get; set; }

        public virtual Cv Cv { get; set; } = null!;
        public virtual JobCriterion MatchedJobCriteria { get; set; } = null!;
    }
}
