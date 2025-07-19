using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Account
    {
        public Account()
        {
            PasswordResetTokens = new HashSet<PasswordResetToken>();
        }

        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual User? User { get; set; }
        public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; }
    }
}
