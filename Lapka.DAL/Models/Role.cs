using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Lapka.DAL.Models
{
    public class Role : IdentityRole
    {
        public Role() { }
        public Role(string roleName) : base(roleName) { }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}

