using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class HandbookCategory
    {
        public HandbookCategory()
        {
            CareerHandbooks = new HashSet<CareerHandbook>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<CareerHandbook> CareerHandbooks { get; set; }
    }
}
