using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class MatchedCvandJobPost
    {
        public MatchedCvandJobPost()
        {
            Cvsubmissions = new HashSet<Cvsubmission>();
        }

        public int MatchedCvandJobPostId { get; set; }
        public int CvparsedDataId { get; set; }
        public int JobPostCriteriaId { get; set; }
        public double? ExperienceScore { get; set; }
        public double? SkillsScore { get; set; }
        public double? EducationLevelScore { get; set; }
        public double? JobTitlesScore { get; set; }
        public double? LanguagesScore { get; set; }
        public double? CertificationsScore { get; set; }
        public double? SummaryScore { get; set; }
        public double? WorkHistoryScore { get; set; }
        public double? ProjectsScore { get; set; }
        public double? TotalScore { get; set; }

        public virtual CvparsedDatum CvparsedData { get; set; } = null!;
        public virtual JobCriterion JobPostCriteria { get; set; } = null!;
        public virtual ICollection<Cvsubmission> Cvsubmissions { get; set; }
    }
}
