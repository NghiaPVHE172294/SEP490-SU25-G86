using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class CompanyFollower
    {
        public int FollowId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public DateTime FlowedAt { get; set; }
        public bool? IsActive { get; set; }

        public virtual Company Company { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
