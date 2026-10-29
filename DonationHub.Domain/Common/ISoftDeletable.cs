using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Domain.Common
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }

        DateTime? DeletedAtUtc { get; set; }

        public void Deleted()
        {
            IsDeleted = true;
            DeletedAtUtc = DateTime.UtcNow;
        }
        public void UndoDelet()
        {
            IsDeleted = false;
            DeletedAtUtc = null;
        }
    }
}
