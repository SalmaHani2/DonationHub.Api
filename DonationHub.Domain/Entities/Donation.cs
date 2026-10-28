using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class Donation : IdentityUser<Guid>
    {
        // Id, CampaignId, DonorUserId, Amount, Status,
        // CreatedAt, CompletedAt?, FailureReason?, ReceiptNumber?, ReceiptFileName?.
        public Guid CampaignId { get; set; }
        public Guid DonorUserId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? ReceiptFileName { get; set; }
    }
}
