using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class JobLevel
    {
        public JobLevel()
        {
            JobPosts = new HashSet<JobPost>();
        }

        public int JobLevelId { get; set; }
        public string JobLevelName { get; set; } = null!;
        public bool IsDelete { get; set; }

        public virtual ICollection<JobPost> JobPosts { get; set; }
    }
}
