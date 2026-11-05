using Microsoft.AspNetCore.Identity;

namespace DonationHub.Application.DTOs.Auth;

public class UserProfileResponse :IdentityUser
{
    //to be more secure insted of ApplicationUser
    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = "en";

    public IList<string> Roles { get; set; } = new List<string>();
}