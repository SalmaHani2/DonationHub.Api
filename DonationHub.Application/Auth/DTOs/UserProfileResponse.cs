
using DonationHub.Domain.Common;

namespace DonationHub.Application.Auth.Auth;

public class UserProfileResponse  :Entity
{
    //to be more secure insted of ApplicationUser
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = "en";

    public IList<string> Roles { get; set; } = new List<string>();
}