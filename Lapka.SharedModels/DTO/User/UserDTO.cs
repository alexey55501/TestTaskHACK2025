using System;

namespace Lapka.SharedModels.DTO.User
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; } // User, Shelter
        public DateTime? Birthday { get; set; }
        public DateTime? RegisterDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }
}

