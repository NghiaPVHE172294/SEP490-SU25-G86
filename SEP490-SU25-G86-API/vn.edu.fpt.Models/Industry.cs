using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Industry
    {
        public Industry()
        {
            Companies = new HashSet<Company>();
            CvTemplates = new HashSet<CvTemplate>();
            JobPositions = new HashSet<JobPosition>();
            JobPosts = new HashSet<JobPost>();
        }

        public int IndustryId { get; set; }
        public string IndustryName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<CvTemplate> CvTemplates { get; set; }
        public virtual ICollection<JobPosition> JobPositions { get; set; }
        public virtual ICollection<JobPost> JobPosts { get; set; }
    }
}
