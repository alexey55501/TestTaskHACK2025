using System.ComponentModel.DataAnnotations;

namespace Lapka.SharedModels.DTO.Auth
{
    public class RestorePasswordDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter correct email.")]
        public string Email { get; set; }
    }
}

