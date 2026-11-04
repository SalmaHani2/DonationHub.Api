using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CampaignConfiguration
    : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CategoryId);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasOne(c => c.Category)
       .WithMany(c => c.Campaigns)
       .HasForeignKey(c => c.CategoryId)
       .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CreatedByUserId);

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.Property(c => c.GoalAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.RaisedAmount)
            .HasPrecision(18, 2);
    }
}