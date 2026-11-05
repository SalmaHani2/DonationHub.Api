
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.PreferredLanguage)
            .IsRequired()
            .HasMaxLength(2)
            .HasDefaultValue("en");

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.HasIndex(u => u.NormalizedEmail)
           .IsUnique()
           .HasFilter("[NormalizedEmail] IS NOT NULL");

        // Indexes
        builder.HasIndex(u => u.Email);
        builder.HasIndex(u => u.UserName);
    }
}
