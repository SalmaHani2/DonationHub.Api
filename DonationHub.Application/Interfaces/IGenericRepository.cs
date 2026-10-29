using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task AddAsync(T entity, CancellationToken cancellationToken);

        Task UpdateAsync(T entity, CancellationToken cancellationToken);

        Task RemoveAsync(T entity, CancellationToken cancellationToken);

        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    }
}
