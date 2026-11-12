using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class Donation : Entity
{
    public Guid CampaignId { get; private set; }
    public Guid DonorUserId { get; private set; }

    public Money Amount { get; private set; } = null!;

    public DonationStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public string? FailureReason { get; private set; }
    public string? ReceiptNumber { get; private set; }
    public string? ReceiptFileName { get; private set; }

    public Campaign Campaign { get; private set; } = null!;

    private Donation()
    {
    }

    public static Donation Create(
        Money amount,
        decimal minAmount,
        decimal maxAmount,
        DateTime currentTime)
    {
        ArgumentNullException.ThrowIfNull(amount);

        if (amount.Amount < minAmount || amount.Amount > maxAmount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Donation amount must be within the allowed range.");
        }

        return new Donation
        {
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

    public void MarkAsFailed(string failureReason, DateTime currentTime)
    {
        if (IsFinalState())
        {
            throw new InvalidOperationException(
                "A final donation state cannot be changed.");
        }

        if (string.IsNullOrWhiteSpace(failureReason))
            throw new ArgumentException("Failure reason is required.");

        Status = DonationStatus.Failed;
        FailureReason = failureReason;
        CompletedAt = currentTime;
    }

    private bool IsFinalState()
    {
        return Status == DonationStatus.Succeeded ||
               Status == DonationStatus.Failed;
    }
}