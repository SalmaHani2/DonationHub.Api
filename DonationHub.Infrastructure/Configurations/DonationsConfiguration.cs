using DonationHub.Domain.Entities;
using DonationHub.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Configurations
{
    public class DonationConfiguration
    : IEntityTypeConfiguration<Donation>
    {
        public void Configure(EntityTypeBuilder<Donation> builder)
        {
            builder.HasOne<Campaign>()
                .WithMany()
                .HasForeignKey(d => d.CampaignId);

            builder.HasIndex(d => d.CampaignId);



            builder.HasOne<ApplicationUser>()
              .WithMany()
               .HasForeignKey(d => d.DonorUserId)
               .OnDelete(DeleteBehavior.Restrict);
 
            builder.HasIndex(d => d.DonorUserId);
        }

    }
}
