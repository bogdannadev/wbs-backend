using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Infrastructure.DataAccess.Postgres.Repositories;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Infrastructure.DataAccess.Postgres;

/// <summary>
/// PostgreSQL implementation of the data service
/// </summary>
public sealed class PostgresDataService : IDataService, IDisposable
{
    private readonly BonusSystemDbContext _dbContext;
    private readonly ILoggerFactory _loggerFactory;
    private bool _disposed;

    public IUserRepository Users { get; }
    public ICompanyRepository Companies { get; }
    public IStoreRepository Stores { get; }
    public ITransactionRepository Transactions { get; }
    public INotificationRepository Notifications { get; }

    public PostgresDataService(
        BonusSystemDbContext dbContext,
        ILoggerFactory loggerFactory)
    {
        _dbContext = dbContext;
        _loggerFactory = loggerFactory;

        // Initialize repositories
        Users = new PostgresUserRepository(dbContext, loggerFactory.CreateLogger<PostgresUserRepository>());

        // For the prototype, we'll use placeholder implementations for other repositories
        // These would be properly implemented for a full solution
        Companies = new PostgresCompanyRepository();
        Stores = new PostgresStoreRepository(dbContext, loggerFactory.CreateLogger<PostgresStoreRepository>());
        Transactions = new PostgresTransactionRepository();
        Notifications = new PostgresNotificationRepository();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _dbContext?.Dispose();
        }

        _disposed = true;
    }
}