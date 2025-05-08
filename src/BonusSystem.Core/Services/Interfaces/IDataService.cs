using BonusSystem.Core.Repositories;

namespace BonusSystem.Core.Services.Interfaces;

/// <summary>
/// Service that provides access to all repositories
/// </summary>
public interface IDataService
{
    IUserRepository Users { get; }
    ICompanyRepository Companies { get; }
    IStoreRepository Stores { get; }
    ITransactionRepository Transactions { get; } 
    IFiatTransactionRepository FiatTransactions { get; }
    INotificationRepository Notifications { get; }
}