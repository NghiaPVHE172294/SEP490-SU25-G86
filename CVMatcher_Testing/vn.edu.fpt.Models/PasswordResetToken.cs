using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class PasswordResetToken
    {
        public int TokenId { get; set; }
        public int AccountId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpireAt { get; set; }
        public bool? IsUsed { get; set; }
        public DateTime CreateAt { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
