using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class SavedJob
    {
        public int SaveJobId { get; set; }
        public int UserId { get; set; }
        public int JobPostId { get; set; }
        public DateTime SaveAt { get; set; }

        public virtual JobPost JobPost { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
