using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class JobPosition
    {
        public JobPosition()
        {
            CvTemplates = new HashSet<CvTemplate>();
            JobPosts = new HashSet<JobPost>();
        }

        public int PositionId { get; set; }
        public int? IndustryId { get; set; }
        public string PostitionName { get; set; } = null!;
        public bool IsDelete { get; set; }

        public virtual Industry? Industry { get; set; }
        public virtual ICollection<CvTemplate> CvTemplates { get; set; }
        public virtual ICollection<JobPost> JobPosts { get; set; }
    }
}
