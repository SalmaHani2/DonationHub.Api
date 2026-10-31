
using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities
{
    public class Campaign : ISoftDeletable
    {
        public Guid Id { get; private set; }
        public string TitleAr { get; set; } = string.Empty;
        public string TitleEn { get; set; } = string.Empty;
        public string DescriptionAr { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;

        public decimal GoalAmount { get; private set; }
        public decimal RaisedAmount { get; private set; }
        public DateTime EndDate { get; private set; }

        public Guid CategoryId { get; set; }
        public string? BannerFileName { get; set; }

        public CampaignStatus Status { get; private set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAtUtc { get; set; }

        private Campaign()
        {
        }

        public static Campaign Create(
            decimal goalAmount,
            DateTime endDate,
            DateTime currentTime)
        {
            if (goalAmount <= 0)
            {
                throw new ArgumentException(
                    "GoalAmount must be greater than zero.");
            }

            if (endDate <= currentTime)
            {
                throw new ArgumentException(
                    "EndDate must be in the future.");
            }

            return new Campaign
            {
                Id = Guid.NewGuid(),
                GoalAmount = goalAmount,
                RaisedAmount = 0,
                EndDate = endDate,
                Status = CampaignStatus.Active
            };
        }
        public void EnsureCanAcceptDonation(DateTime currentTime)
        {
            if (Status != CampaignStatus.Active)
            {
                throw new InvalidOperationException(
                    "Campaign is not active.");
            }

            if (EndDate <= currentTime)
            {
                throw new InvalidOperationException(
                    "Campaign has expired.");
            }
        }
        public void ChangeGoalAmount(decimal goalAmount)
        {
            if (goalAmount <= 0)
            {
                throw new ArgumentException(
                    "GoalAmount must be greater than zero.");
            }

            GoalAmount = goalAmount;
        }

        public void ChangeEndDate(
            DateTime endDate,
            DateTime currentTime)
        {
            if (endDate <= currentTime)
            {
                throw new ArgumentException(
                    "EndDate must be in the future.");
            }

            EndDate = endDate;
        }
        public void AddSucceededDonation(decimal amount)
        {
            if (IsFinalState())
            {
                throw new InvalidOperationException(
                    "A final campaign state cannot be changed.");
            }
            if (amount <= 0)
            {
                throw new ArgumentException(
                    "Donation amount must be greater than zero.");
            }

            RaisedAmount += amount;
            if (RaisedAmount >= GoalAmount)
            {
                Status = CampaignStatus.Completed;
            }
            //GoalAmount cannot be lowered below the current RaisedAmount
            if (GoalAmount < RaisedAmount)
            {
                throw new ArgumentException(
                    "GoalAmount cannot be lower than RaisedAmount.");
            }
        }
        private bool IsFinalState()
        {
            return Status == CampaignStatus.Completed ||
                   Status == CampaignStatus.Expired;
        }
    }
}
