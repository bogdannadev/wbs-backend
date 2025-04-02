using BonusSystem.Infrastructure.DataAccess.Postgres.Entities;
using BonusSystem.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BonusSystem.Infrastructure.DataAccess.Postgres;

public class PostgresInitializer
{
    private readonly BonusSystemDbContext _dbContext;
    private readonly PostgresOptions _options;
    private readonly ILogger<PostgresInitializer> _logger;

    public PostgresInitializer(
        BonusSystemDbContext dbContext,
        IOptions<PostgresOptions> options,
        ILogger<PostgresInitializer> logger)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        try
        {
            if (_options.ApplyMigrationsAtStartup)
            {
                _logger.LogInformation("Applying PostgreSQL migrations...");
                await _dbContext.Database.MigrateAsync();
            }

            await SeedDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    private async Task SeedDataAsync()
    {
        // Only seed if no users exist
        if (await _dbContext.Users.AnyAsync())
        {
            return;
        }

        _logger.LogInformation("Seeding database with initial data...");

        // Create sample users
        var users = new List<UserEntity>
        {
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Username = "buyer1",
                Email = "buyer1@example.com",
                Role = UserRole.Buyer,
                BonusBalance = 25000,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Username = "seller1",
                Email = "seller1@example.com",
                Role = UserRole.Seller,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Username = "admin1",
                Email = "admin1@example.com",
                Role = UserRole.SystemAdmin,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Username = "company1",
                Email = "company1@example.com",
                Role = UserRole.StoreAdmin,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Username = "observer1",
                Email = "observer1@example.com",
                Role = UserRole.CompanyObserver,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        await _dbContext.Users.AddRangeAsync(users);

        // Create sample company
        var company = new CompanyEntity
        {
            Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
            Name = "Sample Company",
            ContactEmail = "contact@samplecompany.com",
            ContactPhone = "+1-555-123-4567",
            BonusBalance = 1000000,
            OriginalBonusBalance = 1000000,
            Status = CompanyStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.Companies.AddAsync(company);

        // Create sample store
        var store = new StoreEntity
        {
            Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
            CompanyId = company.Id,
            Name = "Sample Store",
            Location = "Downtown",
            Address = "123 Main St, Anytown, AN 12345",
            ContactPhone = "+1-555-987-6543",
            Status = StoreStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _dbContext.Stores.AddAsync(store);

        // Create sample store seller assignment
        var storeSellerAssignment = new StoreSellerEntity
        {
            Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
            StoreId = store.Id,
            UserId = users[1].Id, // seller1
            AssignedAt = DateTime.UtcNow
        };

        await _dbContext.StoreSellers.AddAsync(storeSellerAssignment);

        // Create sample transaction
        var transaction = new TransactionEntity
        {
            Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
            UserId = users[0].Id, // buyer1
            CompanyId = company.Id,
            StoreId = store.Id,
            Amount = 100,
            Type = TransactionType.Earn,
            Timestamp = DateTime.UtcNow,
            Status = TransactionStatus.Completed,
            Description = "Sample bonus earning transaction"
        };

        await _dbContext.Transactions.AddAsync(transaction);

        // Create sample notification
        var notification = new NotificationEntity
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            RecipientId = users[0].Id, // buyer1
            Message = "Welcome to the Bonus System!",
            Type = NotificationType.System,
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        await _dbContext.Notifications.AddAsync(notification);

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Database seeded successfully");
    }
}