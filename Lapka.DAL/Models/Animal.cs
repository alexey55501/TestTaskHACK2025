using Lapka.SharedModels.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lapka.API.DAL.Models
{
    public class Animal : AnimalBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual ICollection<UserFavorites> UserFavorites { get; set; }
        public bool IsDeleted { get; set; }
        public int ShelterId { get; set; }
        [ForeignKey("ShelterId")]
        public virtual Shelter Shelter { get; set; }
    }
}

