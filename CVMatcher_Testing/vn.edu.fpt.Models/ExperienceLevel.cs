using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class ExperienceLevel
    {
        public ExperienceLevel()
        {
            JobPosts = new HashSet<JobPost>();
        }

        public int ExperienceLevelId { get; set; }
        public string ExperienceLevelName { get; set; } = null!;
        public int? MinYears { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<JobPost> JobPosts { get; set; }
    }
}
