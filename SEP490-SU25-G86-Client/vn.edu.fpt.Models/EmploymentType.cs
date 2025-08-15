using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class EmploymentType
    {
        public EmploymentType()
        {
            JobPosts = new HashSet<JobPost>();
        }

        public int EmploymentTypeId { get; set; }
        public string EmploymentTypeName { get; set; } = null!;
        public bool IsDelete { get; set; }

        public virtual ICollection<JobPost> JobPosts { get; set; }
    }
}
