using BonusSystem.Core.Repositories;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Infrastructure.DataAccess.InMemory;

/// <summary>
/// In-memory implementation of the user repository
/// </summary>
public class InMemoryUserRepository : InMemoryRepository<UserDto, Guid>, IUserRepository
{
    public InMemoryUserRepository() : base(user => user.Id)
    {
        // Initialize with some sample data
        var users = new List<UserDto>
        {
            new()
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Username = "buyer1",
                Email = "buyer1@example.com",
                Role = UserRole.Buyer,
                BonusBalance = 150
            },
            new()
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Username = "seller1",
                Email = "seller1@example.com",
                Role = UserRole.Seller,
                BonusBalance = 0
            },
            new()
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Username = "admin1",
                Email = "admin1@example.com",
                Role = UserRole.SystemAdmin,
                BonusBalance = 0
            }
        };

        foreach (var user in users)
        {
            _entities[user.Id] = user;
        }
    }

    public Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = _entities.Values.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<bool> UpdateBalanceAsync(Guid userId, decimal newBalance)
    {
        if (!_entities.TryGetValue(userId, out var user))
        {
            return Task.FromResult(false);
        }

        var updatedUser = user with { BonusBalance = newBalance };
        return Task.FromResult(_entities.TryUpdate(userId, updatedUser, user));
    }

    public Task<UserRole> GetUserRoleAsync(Guid userId)
    {
        if (!_entities.TryGetValue(userId, out var user))
        {
            return Task.FromResult(UserRole.Buyer); // Default role
        }

        return Task.FromResult(user.Role);
    }

    public Task<bool> IsUserExistsByEmailAsync(string email)
    {
        var exists = _entities.Values.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(exists);
    }

    public Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role)
    {
        var users = _entities.Values.Where(u => u.Role == role);
        return Task.FromResult(users);
    }
}