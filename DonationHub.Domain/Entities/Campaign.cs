using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class Campaign : Entity, ISoftDeletable
{
    public string TitleAr { get; private set; } = string.Empty;
    public string TitleEn { get; private set; } = string.Empty;
    public string DescriptionAr { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;

    public Money GoalAmount { get; private set; } = null!;
    public Money RaisedAmount { get; private set; } = null!;

    public DateTime EndDate { get; private set; }

    public Guid CategoryId { get; private set; }
    public string? BannerFileName { get; private set; }

    public CampaignStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public Category Category { get; private set; } = null!;
    public ICollection<Donation> Donations { get; private set; } = new List<Donation>();

    public byte[] RowVersion { get; private set; } = null!;

    private Campaign()
    {
    }

    public static Campaign Create(
        Money goalAmount,
        DateTime endDate,
        DateTime currentTime)
    {
        ArgumentNullException.ThrowIfNull(goalAmount);

        if (goalAmount.Amount <= 0)
            throw new ArgumentException("GoalAmount must be greater than zero.");

        if (endDate <= currentTime)
            throw new ArgumentException("EndDate must be in the future.");

        return new Campaign
        {
            GoalAmount = goalAmount,
            RaisedAmount = Money.Create(0m, goalAmount.Currency),
            EndDate = endDate,
            Status = CampaignStatus.Active
        };
    }

    public void EnsureCanAcceptDonation(DateTime currentTime)
    {
        if (Status != CampaignStatus.Active)
            throw new InvalidOperationException("Campaign is not active.");

        if (EndDate <= currentTime)
            throw new InvalidOperationException("Campaign has expired.");
    }

    public void ChangeGoalAmount(Money goalAmount)
    {
        ArgumentNullException.ThrowIfNull(goalAmount);

        if (goalAmount.Amount <= 0)
            throw new ArgumentException("GoalAmount must be greater than zero.");

        if (goalAmount.Currency != RaisedAmount.Currency)
            throw new InvalidOperationException("Currency mismatch.");

        if (goalAmount.Amount < RaisedAmount.Amount)
            throw new ArgumentException("GoalAmount cannot be lower than RaisedAmount.");

        GoalAmount = goalAmount;
    }

    public void ChangeEndDate(DateTime endDate, DateTime currentTime)
    {
        if (endDate <= currentTime)
            throw new ArgumentException("EndDate must be in the future.");

        EndDate = endDate;
    }

    public void AddSucceededDonation(Money amount)
    {
        ArgumentNullException.ThrowIfNull(amount);

        if (IsFinalState())
            throw new InvalidOperationException("A final campaign state cannot be changed.");

        if (amount.Amount <= 0)
            throw new ArgumentException("Donation amount must be greater than zero.");

        if (amount.Currency != RaisedAmount.Currency)
            throw new InvalidOperationException("Currency mismatch.");

        var newRaisedAmount = Money.Create(
            RaisedAmount.Amount + amount.Amount,
            RaisedAmount.Currency);

        RaisedAmount = newRaisedAmount;

        if (RaisedAmount.Amount >= GoalAmount.Amount)
            Status = CampaignStatus.Completed;
    }

    public void Deleted(DateTime deletedAt)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAt;
    }

    public void UndoDelete()
    {
        IsDeleted = false;
        DeletedAtUtc = null;
    }

    private bool IsFinalState()
    {
        return Status == CampaignStatus.Completed ||
               Status == CampaignStatus.Expired;
    }
}