using System.Text.Json.Serialization;

namespace Lapka.SharedModels.Models.Services
{
    public class AuthRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}

