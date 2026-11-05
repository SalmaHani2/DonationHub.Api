using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = "en";
}