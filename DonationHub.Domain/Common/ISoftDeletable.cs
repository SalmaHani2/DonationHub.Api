
namespace DonationHub.Domain.Common;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAtUtc { get; }

    void Deleted(DateTime deletedAt);
    void UndoDelete();
}
