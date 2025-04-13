using Lapka.DAL.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lapka.API.DAL.Models
{
    public class UserFavorites
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public int? AnimalId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Animal Animal { get; set; }
    }
}
