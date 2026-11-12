
using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.HasOne(c => c.Category)
            .WithMany(c => c.Campaigns)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CategoryId);

        builder.HasIndex(c => c.CreatedByUserId);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.OwnsOne(c => c.GoalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("GoalAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("GoalAmountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.OwnsOne(c => c.RaisedAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("RaisedAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("RaisedAmountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}
