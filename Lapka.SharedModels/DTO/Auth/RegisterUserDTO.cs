using Lapka.SharedModels.Base;
using Lapka.SharedModels.DTO.User;
using System.ComponentModel.DataAnnotations;

namespace Lapka.SharedModels.DTO.Auth
{
    public class RegisterUserDTO : UserDTO
    {
        public ShelterType? ShelterType { get; set; }
        public string Address { get; set; }

        [Required(ErrorMessage = "Ім'я обов'язкове.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email обов'язковий.")]
        [EmailAddress(ErrorMessage = "Введіть правильний email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Пароль обов'язковий.")]
        [MinLength(8, ErrorMessage = "Пароль повинен бути не менше 8 символів.")]
        [MaxLength(255, ErrorMessage = "Пароль повинен бути не більше 255 символів.")]
        public string Password { get; set; }
        //[Required(ErrorMessage = "Підтвердження пароля обов'язкове.")]
        //[Compare("Password", ErrorMessage = "Пароль і підтвердження не співпадають.")]
        public string PasswordConfirmation { get; set; }
        [Required(ErrorMessage = "Погодження з умовами та політикою конфіденційності обов'язкове.")]
        [Range(1, 1, ErrorMessage = "Ви повинні погодитися з нашими умовами та політикою конфіденційності, щоб продовжити.")]
        public bool IsAgreedWithTerms { get; set; }
    }
}
