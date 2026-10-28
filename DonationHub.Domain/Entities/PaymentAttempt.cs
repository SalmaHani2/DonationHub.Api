using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class PaymentAttempt : IdentityUser<Guid>
    {
        //Id, DonationId, StripeCheckoutSessionId, StripePaymentIntentId?,
        //Amount, Status, CreatedAt, UpdatedAt, FailureCode?
        public Guid DonationId { get; set; }
        public string StripeCheckoutSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? FailureCode { get; set; }

    }
}
