using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BonusSystem.Infrastructure.DataAccess.InMemory;

/// <summary>
/// In-memory implementation of the data service
/// </summary>
public class InMemoryDataService : IDataService
{
    private readonly AppDbOptions _options;
    
    public IUserRepository Users { get; }
    public ICompanyRepository Companies { get; }
    public IStoreRepository Stores { get; }
    public ITransactionRepository Transactions { get; }
    public INotificationRepository Notifications { get; }

    public InMemoryDataService(IOptions<AppDbOptions> options)
    {
        _options = options.Value;
        
        // Initialize repositories with our implemented classes
        Users = new InMemoryUserRepository();
        Companies = new InMemoryCompanyRepository();
        Stores = new InMemoryStoreRepository();
        Transactions = new InMemoryTransactionRepository();
        Notifications = new InMemoryNotificationRepository();
    }
}