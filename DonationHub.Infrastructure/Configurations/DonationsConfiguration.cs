using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class DonationConfiguration
    : IEntityTypeConfiguration<Donation>
{
    public void Configure(EntityTypeBuilder<Donation> builder)
    {
        builder.HasOne(d => d.Campaign)
            .WithMany(c => c.Donations)
            .HasForeignKey(d => d.CampaignId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.CampaignId);

        builder.HasOne<ApplicationUser>()
            .WithMany(u => u.Donations)
            .HasForeignKey(d => d.DonorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.DonorUserId);

        builder.Property(d => d.Amount)
            .HasPrecision(18, 2);
    }
}