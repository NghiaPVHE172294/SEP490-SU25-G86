using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Notification
    {
        public int NotificationId { get; set; }
        public int ReceiverUserId { get; set; }
        public string Content { get; set; } = null!;
        public string? TargetUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User ReceiverUser { get; set; } = null!;
    }
}
