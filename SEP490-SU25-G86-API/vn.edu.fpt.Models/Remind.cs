using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Remind
    {
        public int RemindId { get; set; }
        public int AdminId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public int UserId { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual User Admin { get; set; } = null!;
    }
}
