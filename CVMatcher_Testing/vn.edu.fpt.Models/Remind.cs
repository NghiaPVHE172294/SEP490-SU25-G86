using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Remind
    {
        public int RemindId { get; set; }
        public string FromEmail { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string ToEmail { get; set; } = null!;
        public DateTime CreateAt { get; set; }
    }
}
