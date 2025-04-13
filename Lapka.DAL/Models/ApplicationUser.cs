using Lapka.API.DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lapka.DAL.Models
{
    public class ApplicationUser : IdentityUser//<string>
    {
        public ApplicationUser()
        {
            UserFavoritesAnimals = new HashSet<UserFavorites>();
        }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime LastActivityDate { get; set; }


        public DateTime? BanDate { get; set; }
        public string BanReason { get; set; }
        [StringLength(256)]
        public string RoleBeforeBan { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("ShelterId")]
        public virtual Shelter? Shelter { get; set; }
        public virtual ICollection<UserFavorites> UserFavoritesAnimals { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        [NotMapped]
        public bool IsBanned => BanDate != null;
    }
}

