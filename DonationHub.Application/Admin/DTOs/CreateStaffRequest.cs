using System.ComponentModel.DataAnnotations;

namespace DonationHub.Application.Admin.Admin;

public class CreateStaffRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string TemporaryPassword { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}