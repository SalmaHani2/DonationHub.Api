using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.DTOs.Auth;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}