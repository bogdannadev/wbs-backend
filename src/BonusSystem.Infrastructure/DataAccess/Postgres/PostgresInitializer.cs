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
            },
            // Additional sellers for new stores
            new()
            {
                Id = Guid.Parse("AAAAAA01-0000-0000-0000-000000000000"),
                Username = "techseller1",
                Email = "techseller1@example.com",
                Role = UserRole.Seller,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("AAAAAA02-0000-0000-0000-000000000000"),
                Username = "techseller2",
                Email = "techseller2@example.com",
                Role = UserRole.Seller,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("AAAAAA03-0000-0000-0000-000000000000"),
                Username = "groceryseller1",
                Email = "groceryseller1@example.com",
                Role = UserRole.Seller,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new()
            {
                Id = Guid.Parse("AAAAAA04-0000-0000-0000-000000000000"),
                Username = "fashionseller1",
                Email = "fashionseller1@example.com",
                Role = UserRole.Seller,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        };

        await _dbContext.Users.AddRangeAsync(users);

        // Create sample companies
        var companies = new List<CompanyEntity>
        {
            new()
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
            },
            // Tech Gadgets Company
            new()
            {
                Id = Guid.Parse("BBBBBB01-0000-0000-0000-000000000000"),
                Name = "TechGadgets Inc.",
                ContactEmail = "contact@techgadgets.com",
                ContactPhone = "+1-555-876-5432",
                BonusBalance = 750000,
                OriginalBonusBalance = 750000,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            // Fresh Groceries Company
            new()
            {
                Id = Guid.Parse("BBBBBB02-0000-0000-0000-000000000000"),
                Name = "Fresh Groceries Co.",
                ContactEmail = "contact@freshgroceries.com",
                ContactPhone = "+1-555-345-6789",
                BonusBalance = 500000,
                OriginalBonusBalance = 500000,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            // Fashion Brands Company
            new()
            {
                Id = Guid.Parse("BBBBBB03-0000-0000-0000-000000000000"),
                Name = "Fashion Brands Collective",
                ContactEmail = "contact@fashionbrands.com",
                ContactPhone = "+1-555-987-6543",
                BonusBalance = 600000,
                OriginalBonusBalance = 600000,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await _dbContext.Companies.AddRangeAsync(companies);
        
        // Reference to first company for existing code
        var company = companies[0];

        // Create sample stores
        var stores = new List<StoreEntity>
        {
            // Original sample stores
            new()
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
            },
            new()
            {
                Id = Guid.Parse("88888888-7777-6666-5555-444444444444"),
                CompanyId = company.Id,
                Name = "Electronics Store",
                Location = "Uptown",
                Address = "456 High St, Anytown, AN 12345",
                ContactPhone = "+1-555-456-7890",
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            
            // TechGadgets stores
            new()
            {
                Id = Guid.Parse("CCCCCC01-0000-0000-0000-000000000000"),
                CompanyId = Guid.Parse("BBBBBB01-0000-0000-0000-000000000000"),
                Name = "TechGadgets Flagship Store",
                Location = "Central Mall",
                Address = "100 Tech Blvd, Techville, TV 54321",
                ContactPhone = "+1-555-111-2222",
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("CCCCCC02-0000-0000-0000-000000000000"),
                CompanyId = Guid.Parse("BBBBBB01-0000-0000-0000-000000000000"),
                Name = "TechGadgets Express",
                Location = "Airport Terminal",
                Address = "200 Airport Way, Techville, TV 54322",
                ContactPhone = "+1-555-333-4444",
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            
            // Fresh Groceries stores
            new()
            {
                Id = Guid.Parse("CCCCCC03-0000-0000-0000-000000000000"),
                CompanyId = Guid.Parse("BBBBBB02-0000-0000-0000-000000000000"),
                Name = "Fresh Groceries Marketplace",
                Location = "Suburban Plaza",
                Address = "300 Fresh Ave, Foodville, FV 65432",
                ContactPhone = "+1-555-555-6666",
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            
            // Fashion Brands stores
            new()
            {
                Id = Guid.Parse("CCCCCC04-0000-0000-0000-000000000000"),
                CompanyId = Guid.Parse("BBBBBB03-0000-0000-0000-000000000000"),
                Name = "Fashion Brands Boutique",
                Location = "Fashion District",
                Address = "400 Style St, Fashionville, FV 87654",
                ContactPhone = "+1-555-777-8888",
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("CCCCCC05-0000-0000-0000-000000000000"),
                CompanyId = Guid.Parse("BBBBBB03-0000-0000-0000-000000000000"),
                Name = "Fashion Brands Outlet",
                Location = "Outlet Mall",
                Address = "500 Discount Dr, Fashionville, FV 87655",
                ContactPhone = "+1-555-999-0000",
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        await _dbContext.Stores.AddRangeAsync(stores);
        
        // Reference to first store for existing code
        var store = stores[0];
        var store2 = stores[1];

        // Create store seller assignments
        var storeSellerAssignments = new List<StoreSellerEntity>
        {
            // Original assignment
            new()
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                StoreId = store.Id,
                UserId = users[1].Id, // seller1
                AssignedAt = DateTime.UtcNow
            },
            
            // TechGadgets seller assignments
            new()
            {
                Id = Guid.Parse("DDDDDD01-0000-0000-0000-000000000000"),
                StoreId = Guid.Parse("CCCCCC01-0000-0000-0000-000000000000"), // TechGadgets Flagship
                UserId = Guid.Parse("AAAAAA01-0000-0000-0000-000000000000"), // techseller1
                AssignedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.Parse("DDDDDD02-0000-0000-0000-000000000000"),
                StoreId = Guid.Parse("CCCCCC02-0000-0000-0000-000000000000"), // TechGadgets Express
                UserId = Guid.Parse("AAAAAA02-0000-0000-0000-000000000000"), // techseller2
                AssignedAt = DateTime.UtcNow
            },
            
            // Fresh Groceries seller assignment
            new()
            {
                Id = Guid.Parse("DDDDDD03-0000-0000-0000-000000000000"),
                StoreId = Guid.Parse("CCCCCC03-0000-0000-0000-000000000000"), // Fresh Groceries Marketplace
                UserId = Guid.Parse("AAAAAA03-0000-0000-0000-000000000000"), // groceryseller1
                AssignedAt = DateTime.UtcNow
            },
            
            // Fashion Brands seller assignment
            new()
            {
                Id = Guid.Parse("DDDDDD04-0000-0000-0000-000000000000"),
                StoreId = Guid.Parse("CCCCCC04-0000-0000-0000-000000000000"), // Fashion Brands Boutique
                UserId = Guid.Parse("AAAAAA04-0000-0000-0000-000000000000"), // fashionseller1
                AssignedAt = DateTime.UtcNow
            }
        };

        await _dbContext.StoreSellers.AddRangeAsync(storeSellerAssignments);

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
        
        // Additional transactions for comprehensive testing
        var additionalTransactions = new List<TransactionEntity>
        {
            // A spending transaction for buyer1
            new()
            {
                Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-111111111111"),
                UserId = users[0].Id, // buyer1
                CompanyId = company.Id,
                StoreId = store.Id,
                Amount = 75,
                Type = TransactionType.Spend,
                Timestamp = DateTime.UtcNow.AddDays(-2),
                Status = TransactionStatus.Completed,
                Description = "Sample bonus spending transaction"
            },
            
            // An expired transaction
            new()
            {
                Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-222222222222"),
                UserId = users[0].Id, // buyer1
                CompanyId = company.Id,
                StoreId = store.Id,
                Amount = 50,
                Type = TransactionType.Expire,
                Timestamp = DateTime.UtcNow.AddDays(-10),
                Status = TransactionStatus.Completed,
                Description = "Expired bonus points"
            },
            
            // A transaction for seller1
            new()
            {
                Id = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-333333333333"),
                UserId = users[1].Id, // seller1
                CompanyId = company.Id,
                StoreId = store.Id,
                Amount = 200,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-5),
                Status = TransactionStatus.Completed,
                Description = "Seller bonus transaction"
            }
        };

        await _dbContext.Transactions.AddRangeAsync(additionalTransactions);

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
        
        // Create second notification
        var notification2 = new NotificationEntity
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            RecipientId = users[0].Id, // buyer1
            Message = "Your bonus points will expire in 30 days",
            Type = NotificationType.Expiration,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            IsRead = true
        };

        await _dbContext.Notifications.AddAsync(notification2);

        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Database seeded successfully");
    }
}