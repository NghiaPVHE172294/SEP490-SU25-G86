using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class JobCriterion
    {
        public JobCriterion()
        {
            MatchedCvandJobPosts = new HashSet<MatchedCvandJobPost>();
        }

        public int JobCriteriaId { get; set; }
        public int JobPostId { get; set; }
        public double? RequiredExperience { get; set; }
        public string? RequiredSkills { get; set; }
        public string? EducationLevel { get; set; }
        public string? RequiredJobTitles { get; set; }
        public string? PreferredLanguages { get; set; }
        public string? PreferredCertifications { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public string? Address { get; set; }
        public string? Summary { get; set; }
        public string? WorkHistory { get; set; }
        public string? Projects { get; set; }
        public bool IsDelete { get; set; }

        public virtual User CreatedByUser { get; set; } = null!;
        public virtual ICollection<MatchedCvandJobPost> MatchedCvandJobPosts { get; set; }
    }
}
