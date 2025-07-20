using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Role
    {
        public Role()
        {
            Accounts = new HashSet<Account>();
            RolePermissions = new HashSet<RolePermission>();
        }

        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
