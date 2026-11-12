
using DonationHub.Domain.Common;

namespace DonationHub.Domain.Entities;

public class Category : Entity, ISoftDeletable
{
    public string? NameAr { get; private set; }
    public string? NameEn { get; private set; }
    public string? DescriptionAr { get; private set; }
    public string? DescriptionEn { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public ICollection<Campaign> Campaigns { get; private set; }
        = new List<Campaign>();

    private Category() { }

    public static Category Create(
        string nameAr,
        string nameEn,
        string? descriptionAr,
        string? descriptionEn,
        DateTime currentTime)
    {
        return new Category
        {
            NameAr = nameAr,
            NameEn = nameEn,
            DescriptionAr = descriptionAr,
            DescriptionEn = descriptionEn,
            CreatedAt = currentTime,
            UpdatedAt = currentTime
        };
    }

    public void Update(
        string nameAr,
        string nameEn,
        string? descriptionAr,
        string? descriptionEn,
        DateTime currentTime)
    {
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        UpdatedAt = currentTime;
    }

    public void EnsureCanBeDeleted()
    {
        if (Campaigns.Any())
            throw new InvalidOperationException("Category.HasCampaigns");
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
}
