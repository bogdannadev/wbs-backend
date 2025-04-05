using BonusSystem.Infrastructure.DataAccess.Entities;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Seeding.SeedData;

internal class StoreSeedData
{
    public static IEnumerable<StoreEntity> GetStores()
    {
        return new List<StoreEntity>
        {
            // MegaMart Stores
            new StoreEntity
            {
                Id = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
                CompanyId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                Name = "MegaMart Downtown",
                Location = "Downtown",
                Address = "123 Main Street, Metropolis, NY 10001",
                ContactPhone = "+1-555-111-2222",
                Category = "Supermarket",
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-58),
                UpdatedAt = DateTime.UtcNow.AddDays(-14)
            },
            new StoreEntity
            {
                Id = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"),
                CompanyId = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                Name = "MegaMart Uptown",
                Location = "Uptown",
                Address = "456 High Street, Metropolis, NY 10002",
                ContactPhone = "+1-555-333-4444",
                Category = "Supermarket",
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-55),
                UpdatedAt = DateTime.UtcNow.AddDays(-12)
            },

            // TechHaven Stores
            new StoreEntity
            {
                Id = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF"),
                CompanyId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Name = "TechHaven Mall Location",
                Location = "City Mall",
                Address = "789 Mall Road, Metropolis, NY 10003",
                ContactPhone = "+1-555-555-6666",
                Category = "Electronics",
                Status = StoreStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-42),
                UpdatedAt = DateTime.UtcNow.AddDays(-8)
            },

            // FreshFoods Stores
            new StoreEntity
            {
                Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAB"),
                CompanyId = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
                Name = "FreshFoods Market Central",
                Location = "Central District",
                Address = "321 Fresh Street, Metropolis, NY 10004",
                ContactPhone = "+1-555-777-8888",
                Category = "Grocery",
                Status = StoreStatus.PendingApproval,
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            }
        };
    }
}