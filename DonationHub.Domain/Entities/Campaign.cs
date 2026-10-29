using DonationHub.Domain.Common;
using DonationHub.Domain.ValueObject;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class Campaign : IdentityUser<Guid>, ISoftDeletable
    {
        //Id, TitleAr, TitleEn, DescriptionAr, DescriptionEn, GoalAmount, RaisedAmount,EndDate,
        //CategoryId, BannerFileName?, Status, CreatedByUserId, CreatedAt, UpdatedAt
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public decimal GoalAmount { get; set; }
        public decimal RaisedAmount { get; set; }
        public DateTime EndDate { get; set; }
        public Guid CategoryId { get; set; }
        public string? BannerFileName { get; set; }
        public string Status { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAtUtc { get; set; }
    }
}
