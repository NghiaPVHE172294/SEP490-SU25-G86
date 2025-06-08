using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool? IsRead { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
