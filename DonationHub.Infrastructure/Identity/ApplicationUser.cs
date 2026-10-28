using Microsoft.AspNetCore.Identity;
namespace DonationHub.Domain.ValueObject;
public class ApplicationUser : IdentityUser<Guid>
{
    //ApplicationUser(Identity user) : Id, Email, UserName, FullName,
    //PreferredLanguage(en | ar, default en), IsActive, CreatedAt.
    //Roles: Admin, CampaignManager, Donor.
    public string FullName { get; private set; } = string.Empty;

    public string PreferredLanguage { get; private set; } = "en";

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAt { get; private set; }
}