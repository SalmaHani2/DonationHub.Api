using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class ProcessedStripeEvent : IdentityUser<Guid>
    {
        //EventId (key), Type, ProcessedAt.
        public string EventId { get; set; }
        public string Type { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
