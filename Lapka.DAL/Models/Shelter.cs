using Lapka.DAL.Models;
using Lapka.SharedModels.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lapka.API.DAL.Models
{
    public class Shelter : ShelterBase
    {
        public Shelter()
        {
            Animals = new HashSet<Animal>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual ICollection<Animal> Animals { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime AdditionDate { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}

