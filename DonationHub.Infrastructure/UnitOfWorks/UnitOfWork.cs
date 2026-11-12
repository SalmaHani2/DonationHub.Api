using DonationHub.Application.Common;
using DonationHub.Domain.Common;
using DonationHub.Infrastructure.Persistence;
using DonationHub.Infrastructure.Repositories;


namespace DonationHub.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWorks
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repository<T>() where T : Entity
        {
            var type = typeof(T);

            if (!_repositories.TryGetValue(type, out var repository))
            {
                repository = new GenericRepository<T>(_context);
                _repositories[type] = repository;
            }

            return (IGenericRepository<T>)repository;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}