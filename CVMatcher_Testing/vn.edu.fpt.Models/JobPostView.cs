using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class JobPostView
    {
        public int ViewId { get; set; }
        public int JobPostId { get; set; }
        public int UserId { get; set; }
        public DateTime ViewAt { get; set; }

        public virtual JobPost JobPost { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
