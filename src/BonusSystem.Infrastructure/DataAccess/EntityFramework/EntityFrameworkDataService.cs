using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Infrastructure.DataAccess.EntityFramework.Repositories;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Infrastructure.DataAccess.EntityFramework;

public class EntityFrameworkDataService : IDataService
{
    private readonly BonusSystemContext _context;
    private readonly ILoggerFactory _loggerFactory;

    public EntityFrameworkDataService(BonusSystemContext context, ILoggerFactory loggerFactory)
    {
        _context = context;
        _loggerFactory = loggerFactory;
        
        
        Users = new EntityFrameworkUserRepository(_context, _loggerFactory.CreateLogger<EntityFrameworkUserRepository>());
        Companies = new EntityFrameworkCompanyRepository(_context, _loggerFactory.CreateLogger<EntityFrameworkCompanyRepository>());
        Stores = new EntityFrameworkStoreRepository(_context, _loggerFactory.CreateLogger<EntityFrameworkStoreRepository>());
        Transactions = new EntityFrameworkTransactionRepository(_context, _loggerFactory.CreateLogger<EntityFrameworkTransactionRepository>());
        Notifications = new EntityFrameworkNotificationRepository(_context, _loggerFactory.CreateLogger<EntityFrameworkNotificationRepository>());
    }

    public IUserRepository Users { get; }
    public ICompanyRepository Companies { get; }
    public IStoreRepository Stores { get; }
    public ITransactionRepository Transactions { get; }
    public INotificationRepository Notifications { get; }
}