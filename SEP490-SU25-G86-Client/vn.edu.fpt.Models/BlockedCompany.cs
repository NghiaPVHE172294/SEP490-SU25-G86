using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class BlockedCompany
    {
        public int BlockedCompaniesId { get; set; }
        public int CompanyId { get; set; }
        public int CandidateId { get; set; }
        public string? Reason { get; set; }

        public virtual User Candidate { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
    }
}
