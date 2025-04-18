using BonusSystem.Infrastructure.DataAccess.Entities;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Seeding.SeedData;

internal static class UserSeedData
{
    public static IEnumerable<UserEntity> GetUsers()
    {
        return new List<UserEntity>
        {
            // System Administrator
            new UserEntity
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Username = "admin",
                Email = "admin@bonussystem.com",
                Role = UserRole.SystemAdmin,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastLogin = DateTime.UtcNow.AddDays(-1),
                IsActive = true
            },

            // Company Observer
            new UserEntity
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Username = "observer",
                Email = "observer@megamart.com",
                Role = UserRole.CompanyObserver,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                LastLogin = DateTime.UtcNow.AddDays(-2),
                IsActive = true
            },

            // Company Users (one for each company)
            new UserEntity
            {
                Id = Guid.Parse("AAAAABBB-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                Username = "megamart",
                Email = "company@megamart.com",
                Role = UserRole.Company,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                LastLogin = DateTime.UtcNow.AddDays(-1),
                IsActive = true
            },
            new UserEntity
            {
                Id = Guid.Parse("BBBBBCCC-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Username = "techhaven",
                Email = "company@techhaven.com",
                Role = UserRole.Company,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-45),
                LastLogin = DateTime.UtcNow.AddDays(-2),
                IsActive = true
            },
            new UserEntity
            {
                Id = Guid.Parse("CCCCCAAA-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
                Username = "freshfoods",
                Email = "company@freshfoods.com",
                Role = UserRole.Company,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                LastLogin = DateTime.UtcNow.AddDays(-1),
                IsActive = true
            },
            new UserEntity
            {
                Id = Guid.Parse("DDDDDEEE-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
                Username = "fashionboutique",
                Email = "company@fashionboutique.com",
                Role = UserRole.Company,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastLogin = DateTime.UtcNow.AddDays(-1),
                IsActive = true
            },
            new UserEntity
            {
                Id = Guid.Parse("EEEEEFFF-EEEE-EEEE-EEEE-EEEEEEEEEEEE"),
                Username = "familypharmacy",
                Email = "company@familypharmacy.com",
                Role = UserRole.Company,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-75),
                LastLogin = DateTime.UtcNow.AddDays(-3),
                IsActive = true
            },

            // Store Admin
            new UserEntity
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Username = "storeadmin",
                Email = "storeadmin@megamart.com",
                Role = UserRole.StoreAdmin,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                LastLogin = DateTime.UtcNow.AddDays(-1),
                IsActive = true
            },

            // Sellers (store employees)
            new UserEntity
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Username = "seller1",
                Email = "seller1@megamart.com",
                Role = UserRole.Seller,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                LastLogin = DateTime.UtcNow.AddHours(-4),
                IsActive = true
            },
            new UserEntity
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Username = "seller2",
                Email = "seller2@megamart.com",
                Role = UserRole.Seller,
                BonusBalance = 0,
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                LastLogin = DateTime.UtcNow.AddHours(-2),
                IsActive = true
            },

            // Buyers (customers)
            new UserEntity
            {
                Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                Username = "customer1",
                Email = "customer1@example.com",
                Role = UserRole.Buyer,
                BonusBalance = 1250.50m,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                LastLogin = DateTime.UtcNow.AddHours(-1),
                IsActive = true
            },
            new UserEntity
            {
                Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                Username = "customer2",
                Email = "customer2@example.com",
                Role = UserRole.Buyer,
                BonusBalance = 750.25m,
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                LastLogin = DateTime.UtcNow.AddHours(-5),
                IsActive = true
            },
            new UserEntity
            {
                Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                Username = "customer3",
                Email = "customer3@example.com",
                Role = UserRole.Buyer,
                BonusBalance = 2000.00m,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                LastLogin = DateTime.UtcNow.AddHours(-3),
                IsActive = true
            }
        };
    }
}