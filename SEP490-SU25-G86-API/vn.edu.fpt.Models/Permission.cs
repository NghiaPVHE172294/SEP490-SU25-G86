using System;
using System.Collections.Generic;

namespace SEP490_SU25_G86_API.Models
{
    public partial class Permission
    {
        public Permission()
        {
            RolePermissions = new HashSet<RolePermission>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Method { get; set; } = null!;
        public string Endpoint { get; set; } = null!;

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
