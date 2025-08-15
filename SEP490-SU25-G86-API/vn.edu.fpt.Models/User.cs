using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class User
    {
        public User()
        {
            AuditLogs = new HashSet<AuditLog>();
            BlockedCompanies = new HashSet<BlockedCompany>();
            CompanyFollowers = new HashSet<CompanyFollower>();
            CvTemplateOfEmployers = new HashSet<CvTemplateOfEmployer>();
            CvTemplates = new HashSet<CvTemplate>();
            Cvs = new HashSet<Cv>();
            Cvsubmissions = new HashSet<Cvsubmission>();
            JobCriteria = new HashSet<JobCriterion>();
            JobPostViews = new HashSet<JobPostView>();
            JobPosts = new HashSet<JobPost>();
            Notifications = new HashSet<Notification>();
            RequireOfCompanies = new HashSet<RequireOfCompany>();
            SavedJobs = new HashSet<SavedJob>();
        }

        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? LinkedIn { get; set; }
        public string? Facebook { get; set; }
        public string? AboutMe { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CompanyId { get; set; }
        public bool? IsActive { get; set; }
        public int AccountId { get; set; }
        public bool IsBan { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Company? Company { get; set; }
        public virtual ICollection<AuditLog> AuditLogs { get; set; }
        public virtual ICollection<BlockedCompany> BlockedCompanies { get; set; }
        public virtual ICollection<CompanyFollower> CompanyFollowers { get; set; }
        public virtual ICollection<CvTemplateOfEmployer> CvTemplateOfEmployers { get; set; }
        public virtual ICollection<CvTemplate> CvTemplates { get; set; }
        public virtual ICollection<Cv> Cvs { get; set; }
        public virtual ICollection<Cvsubmission> Cvsubmissions { get; set; }
        public virtual ICollection<JobCriterion> JobCriteria { get; set; }
        public virtual ICollection<JobPostView> JobPostViews { get; set; }
        public virtual ICollection<JobPost> JobPosts { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<RequireOfCompany> RequireOfCompanies { get; set; }
        public virtual ICollection<SavedJob> SavedJobs { get; set; }
    }
}
