using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities
{
    public class Donation
    {
        public Guid Id { get; private set; }

        public Guid CampaignId { get; set; }
        public Guid DonorUserId { get; set; }

        public decimal Amount { get; private set; }

        public DonationStatus Status { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public string? FailureReason { get; private set; }
        public string? ReceiptNumber { get; private set; }
        public string? ReceiptFileName { get; private set; }

        private Donation()
        {
        }

        public static Donation Create(
            decimal amount,
            decimal minAmount,
            decimal maxAmount,
            DateTime currentTime)
        {
            if (amount < minAmount || amount > maxAmount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Donation amount must be within the allowed range.");
            }

            return new Donation
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                Status = DonationStatus.Pending,
                CreatedAt = currentTime
            };
        }
        public void MarkAsSucceeded(DateTime currentTime)
        {
            if (IsFinalState())
            {
                throw new InvalidOperationException(
                    "A final donation state cannot be changed.");
            }

            Status = DonationStatus.Succeeded;
            CompletedAt = currentTime;
        }

        private bool IsFinalState()
        {
            return Status == DonationStatus.Succeeded ||
                   Status == DonationStatus.Failed;
        }
    }
}