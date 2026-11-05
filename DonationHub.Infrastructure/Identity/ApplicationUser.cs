using DonationHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;

    public string PreferredLanguage { get; set; } = "en";

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public ApplicationUser()
    {
    }
    public ApplicationUser(string email,string fullName,string preferredLanguage,DateTime createdAt)
    {
        Email = email;
        UserName = email;
        FullName = fullName;
        PreferredLanguage = preferredLanguage;
        CreatedAt = createdAt;
        IsActive = true;
    }
}