using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class JobPost
    {
        public JobPost()
        {
            CvTemplateForJobposts = new HashSet<CvTemplateForJobpost>();
            Cvsubmissions = new HashSet<Cvsubmission>();
            JobPostViews = new HashSet<JobPostView>();
            SavedJobs = new HashSet<SavedJob>();
        }

        public int JobPostId { get; set; }
        public int? EmployerId { get; set; }
        public int? IndustryId { get; set; }
        public int? JobPositionId { get; set; }
        public string Title { get; set; } = null!;
        public int? SalaryRangeId { get; set; }
        public int? ProvinceId { get; set; }
        public int? ExperienceLevelId { get; set; }
        public int? JobLevelId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public string? CandidaterRequirements { get; set; }
        public string? Interest { get; set; }
        public string? WorkLocation { get; set; }
        public bool IsAienabled { get; set; }
        public string? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDelete { get; set; }

        public virtual User? Employer { get; set; }
        public virtual EmploymentType? EmploymentType { get; set; }
        public virtual ExperienceLevel? ExperienceLevel { get; set; }
        public virtual Industry? Industry { get; set; }
        public virtual JobLevel? JobLevel { get; set; }
        public virtual JobPosition? JobPosition { get; set; }
        public virtual Province? Province { get; set; }
        public virtual SalaryRange? SalaryRange { get; set; }
        public virtual ICollection<CvTemplateForJobpost> CvTemplateForJobposts { get; set; }
        public virtual ICollection<Cvsubmission> Cvsubmissions { get; set; }
        public virtual ICollection<JobPostView> JobPostViews { get; set; }
        public virtual ICollection<SavedJob> SavedJobs { get; set; }
    }
}
