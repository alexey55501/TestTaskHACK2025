using System;

namespace Lapka.SharedModels.DTO.User
{
    public class UserShortInfoDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public DateTime LastActivityDate { get; set; }
    }
}

