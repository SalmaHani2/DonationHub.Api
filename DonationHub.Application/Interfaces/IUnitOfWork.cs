using DonationHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>() where T : Entity;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
