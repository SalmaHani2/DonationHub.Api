using DonationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class ProcessedStripeEvent : Entity
    {
        //EventId (key), Type, ProcessedAt.
        public string EventId { get;private set; }
        public string Type { get; private set; }
        public DateTime ProcessedAt { get; private set; }
    }
}
