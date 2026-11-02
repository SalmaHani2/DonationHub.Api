using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.DTOs.Auth
{
    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string Password { get; set; }
    }
}
