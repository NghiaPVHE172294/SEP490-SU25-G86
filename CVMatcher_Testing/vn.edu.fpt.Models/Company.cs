using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Company
    {
        public Company()
        {
            BlockedCompanies = new HashSet<BlockedCompany>();
            CompanyFollowers = new HashSet<CompanyFollower>();
            RequireOfCompanies = new HashSet<RequireOfCompany>();
            Users = new HashSet<User>();
        }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string TaxCode { get; set; } = null!;
        public int IndustryId { get; set; }
        public string Email { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Website { get; set; }
        public string CompanySize { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsDelete { get; set; }
        public bool Status { get; set; }

        public virtual Industry Industry { get; set; } = null!;
        public virtual ICollection<BlockedCompany> BlockedCompanies { get; set; }
        public virtual ICollection<CompanyFollower> CompanyFollowers { get; set; }
        public virtual ICollection<RequireOfCompany> RequireOfCompanies { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
