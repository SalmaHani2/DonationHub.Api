using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class PaymentAttempt : Entity
    {
        //Id, DonationId, StripeCheckoutSessionId, StripePaymentIntentId?,
        //Amount, Status, CreatedAt, UpdatedAt, FailureCode?
        public Guid DonationId { get; private set; }
        public string StripeCheckoutSessionId { get; private set; }
        public string? StripePaymentIntentId { get; private set; }
        public Money Amount { get; private set; }
        public PaymentAttemptStatus Status { get; private set; }
        public DateTime CreatedAt { get; private  set; }
        public DateTime UpdatedAt { get; private set; }
        public string? FailureCode { get; private set; }

    }
}
