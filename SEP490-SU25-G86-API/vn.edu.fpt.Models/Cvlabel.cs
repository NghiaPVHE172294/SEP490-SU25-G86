using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Cvlabel
    {
        public Cvlabel()
        {
            Cvsubmissions = new HashSet<Cvsubmission>();
        }

        public int LabelId { get; set; }
        public string LabelName { get; set; } = null!;
        public string? ColorCode { get; set; }
        public string? Description { get; set; }
        public bool IsDelete { get; set; }

        public virtual ICollection<Cvsubmission> Cvsubmissions { get; set; }
    }
}
