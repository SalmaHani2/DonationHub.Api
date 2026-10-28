using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Entities
{
    public class DonationOptions
    {
        public decimal MinimumAmount { get; init; } = 1.00m;

        public decimal MaximumAmount { get; init; } = 10_000.00m;
    }
}
