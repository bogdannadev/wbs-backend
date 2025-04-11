using BonusSystem.Infrastructure.DataAccess.Entities;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Seeding.SeedData;

internal class TransactionSeedData
{
    public static IEnumerable<BonusTransactionEntity> GetTransactions()
    {
        return new List<BonusTransactionEntity>
        {
            // Transactions for customer1 at MegaMart Downtown
            new BonusTransactionEntity
            {
                Id = Guid.Parse("11111111-AAAA-1111-AAAA-111111111111"),
                UserId = Guid.Parse("66666666-6666-6666-6666-666666666666"), // customer1
                CompanyId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"), // MegaMart
                StoreId = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"), // MegaMart Downtown
                BonusAmount = 500.00m,
                TotalCost = 1500.00m,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-7),
                Status = TransactionStatus.Completed,
                Description = "Bonus points for purchase"
            },
            new BonusTransactionEntity
            {
                Id = Guid.Parse("22222222-AAAA-2222-AAAA-222222222222"),
                UserId = Guid.Parse("66666666-6666-6666-6666-666666666666"), // customer1
                CompanyId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"), // MegaMart
                StoreId = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"), // MegaMart Downtown
                BonusAmount = 250.00m,
                TotalCost = 1250.00m,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-5),
                Status = TransactionStatus.Completed,
                Description = "Bonus points for purchase"
            },
            new BonusTransactionEntity
            {
                Id = Guid.Parse("33333333-AAAA-3333-AAAA-333333333333"),
                UserId = Guid.Parse("66666666-6666-6666-6666-666666666666"), // customer1
                CompanyId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"), // MegaMart
                StoreId = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"), // MegaMart Downtown
                BonusAmount = 200.00m,
                TotalCost = 1200.00m,
                Type = TransactionType.Spend,
                Timestamp = DateTime.UtcNow.AddDays(-2),
                Status = TransactionStatus.Completed,
                Description = "Redeemed bonus points"
            },

            // Transactions for customer2 at MegaMart Uptown
            new BonusTransactionEntity
            {
                Id = Guid.Parse("44444444-AAAA-4444-AAAA-444444444444"),
                UserId = Guid.Parse("77777777-7777-7777-7777-777777777777"), // customer2
                CompanyId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"), // MegaMart
                StoreId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"), // MegaMart Uptown
                BonusAmount = 350.00m,
                TotalCost = 1350.00m,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-6),
                Status = TransactionStatus.Completed,
                Description = "Bonus points for purchase"
            },
            new BonusTransactionEntity
            {
                Id = Guid.Parse("55555555-AAAA-5555-AAAA-555555555555"),
                UserId = Guid.Parse("77777777-7777-7777-7777-777777777777"), // customer2
                CompanyId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"), // MegaMart
                StoreId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"), // MegaMart Uptown
                BonusAmount = 150.00m,
                TotalCost = 1150.00m,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-4),
                Status = TransactionStatus.Completed,
                Description = "Bonus points for purchase"
            },
            new BonusTransactionEntity
            {
                Id = Guid.Parse("66666666-AAAA-6666-AAAA-666666666666"),
                UserId = Guid.Parse("77777777-7777-7777-7777-777777777777"), // customer2
                CompanyId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"), // MegaMart
                StoreId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"), // MegaMart Uptown
                BonusAmount = 100.00m,
                TotalCost = 1100.00m,
                Type = TransactionType.Spend,
                Timestamp = DateTime.UtcNow.AddDays(-1),
                Status = TransactionStatus.Completed,
                Description = "Redeemed bonus points"
            },

            // Transactions for customer3 at TechHaven
            new BonusTransactionEntity
            {
                Id = Guid.Parse("77777777-AAAA-7777-AAAA-777777777777"),
                UserId = Guid.Parse("88888888-8888-8888-8888-888888888888"), // customer3
                CompanyId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"), // TechHaven
                StoreId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"), // TechHaven Mall
                BonusAmount = 1500.00m,
                TotalCost = 2500.00m,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-3),
                Status = TransactionStatus.Completed,
                Description = "Bonus points for purchase"
            },
            new BonusTransactionEntity
            {
                Id = Guid.Parse("88888888-AAAA-8888-AAAA-888888888888"),
                UserId = Guid.Parse("88888888-8888-8888-8888-888888888888"), // customer3
                CompanyId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"), // TechHaven
                StoreId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"), // TechHaven Mall
                BonusAmount = 200.00m,
                TotalCost = 1200.00m,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-2),
                Status = TransactionStatus.Completed,
                Description = "Bonus points for purchase"
            },
            new BonusTransactionEntity
            {
                Id = Guid.Parse("99999999-AAAA-9999-AAAA-999999999999"),
                UserId = Guid.Parse("88888888-8888-8888-8888-888888888888"), // customer3
                CompanyId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"), // TechHaven
                StoreId = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"), // TechHaven Mall
                BonusAmount = 300.00m,
                TotalCost = 1300.00m,
                Type = TransactionType.Spend,
                Timestamp = DateTime.UtcNow.AddDays(-1).AddHours(-6),
                Status = TransactionStatus.Completed,
                Description = "Redeemed bonus points"
            }
        };
    }
}