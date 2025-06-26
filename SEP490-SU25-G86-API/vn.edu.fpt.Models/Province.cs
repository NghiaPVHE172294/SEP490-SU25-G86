using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Province
    {
        public Province()
        {
            JobPosts = new HashSet<JobPost>();
        }

        public int ProvinceId { get; set; }
        public string ProvinceName { get; set; } = null!;
        public string? Region { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<JobPost> JobPosts { get; set; }
    }
}
