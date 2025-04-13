﻿using System;
using System.Collections.Generic;

namespace Lapka.SharedModels.DTO.Auth
{
    public class AuthenticatedUserResponseDTO
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public List<string> Roles { get; set; }
    }
}
