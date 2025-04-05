using BonusSystem.Infrastructure.DataAccess.Entities;

namespace BonusSystem.Infrastructure.DataAccess.Seeding.SeedData;

internal class StoreAssignmentSeedData
{
    public static IEnumerable<StoreSellerAssignmentEntity> GetStoreSellers()
    {
        return new List<StoreSellerAssignmentEntity>
        {
            // Assign seller1 to MegaMart Downtown
            new StoreSellerAssignmentEntity
            {
                Id = Guid.Parse("11111111-AAAA-BBBB-CCCC-DDDDDDDDDDDD"),
                StoreId = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD"),
                UserId = Guid.Parse("44444444-4444-4444-4444-444444444444"), // seller1
                AssignedAt = DateTime.UtcNow.AddDays(-14)
            },

            // Assign seller2 to MegaMart Uptown
            new StoreSellerAssignmentEntity
            {
                Id = Guid.Parse("22222222-AAAA-BBBB-CCCC-DDDDDDDDDDDD"),
                StoreId = Guid.Parse("EEEEEEEE-EEEE-EEEE-EEEE-EEEEEEEEEEEE"),
                UserId = Guid.Parse("55555555-5555-5555-5555-555555555555"), // seller2
                AssignedAt = DateTime.UtcNow.AddDays(-13)
            }
        };
    }
}