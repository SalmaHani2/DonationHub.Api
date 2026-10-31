using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class ProcessedStripeEvent 
    {
        //EventId (key), Type, ProcessedAt.
        public Guid Id { get; set; }
        public string EventId { get; set; }
        public string Type { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
