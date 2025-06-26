using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class SalaryRange
    {
        public SalaryRange()
        {
            JobPosts = new HashSet<JobPost>();
        }

        public int SalaryRangeId { get; set; }
        public int? MinSalary { get; set; }
        public int? MaxSalary { get; set; }
        public string? Currency { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<JobPost> JobPosts { get; set; }
    }
}
