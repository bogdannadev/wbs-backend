using BonusSystem.Infrastructure.DataAccess.Entities;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.Seeding.SeedData;

internal class CompanySeedData
{
    public static IEnumerable<CompanyEntity> GetCompanies()
    {
        return new List<CompanyEntity>
        {
            new CompanyEntity
            {
                Id = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"),
                Name = "MegaMart Retail",
                ContactEmail = "contact@megamart.com",
                ContactPhone = "+1-555-123-4567",
                BonusBalance = 250000.00m,
                OriginalBonusBalance = 500000.00m,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                UpdatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new CompanyEntity
            {
                Id = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"),
                Name = "TechHaven Electronics",
                ContactEmail = "info@techhaven.com",
                ContactPhone = "+1-555-987-6543",
                BonusBalance = 150000.00m,
                OriginalBonusBalance = 200000.00m,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow.AddDays(-45),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            },
            new CompanyEntity
            {
                Id = Guid.Parse("CCCCCCCC-CCCC-CCCC-CCCC-CCCCCCCCCCCC"),
                Name = "FreshFoods Market",
                ContactEmail = "support@freshfoods.com",
                ContactPhone = "+1-555-789-0123",
                BonusBalance = 100000.00m,
                OriginalBonusBalance = 100000.00m,
                Status = CompanyStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            }
        };
    }
}