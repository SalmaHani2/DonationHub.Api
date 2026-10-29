using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class CampaignConfiguration
    : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CategoryId);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasOne<ApplicationUser>()
    .WithMany()
    .HasForeignKey(c => c.CreatedByUserId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CreatedByUserId);
    }
}