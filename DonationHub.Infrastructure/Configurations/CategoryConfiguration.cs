using DonationHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DonationHub.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.NameAr)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.NameEn)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(x => x.NameAr)
            .IsUnique();

        builder.HasIndex(x => x.NameEn)
            .IsUnique();

        builder.HasMany(x => x.Campaigns)
            .WithOne()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}