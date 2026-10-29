using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> Repository<T>()
        where T : class;

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
