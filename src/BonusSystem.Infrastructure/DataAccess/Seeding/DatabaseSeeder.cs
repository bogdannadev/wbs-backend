using BonusSystem.Infrastructure.DataAccess.EntityFramework;
using BonusSystem.Infrastructure.DataAccess.Seeding.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Infrastructure.DataAccess.Seeding;

public class DatabaseSeeder
{
    private readonly BonusSystemContext _dbContext;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(BonusSystemContext dbContext, ILogger<DatabaseSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            if (await _dbContext.Users.AnyAsync())
            {
                _logger.LogInformation("Database already contains data, skipping seeding");
                return;
            }
            
            _logger.LogInformation("Starting database seeding...");
            
            // Seed data in transaction
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            
            await _dbContext.Users.AddRangeAsync(UserSeedData.GetUsers());
            await _dbContext.Companies.AddRangeAsync(CompanySeedData.GetCompanies());
            await _dbContext.Stores.AddRangeAsync(StoreSeedData.GetStores());
            await _dbContext.StoreSellerAssignments.AddRangeAsync(StoreAssignmentSeedData.GetStoreSellers());
            await _dbContext.BonusTransactions.AddRangeAsync(TransactionSeedData.GetTransactions());

            await _dbContext.SaveChangesAsync();
            
            await transaction.CommitAsync();
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error during database seeding");
            throw;
        }
    }
}