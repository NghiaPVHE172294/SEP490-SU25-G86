using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class AuditLog
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public string? Action { get; set; }
        public string? TargetTable { get; set; }
        public int? TargetId { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
