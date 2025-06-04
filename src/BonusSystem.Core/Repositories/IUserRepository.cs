using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Core.Repositories;

/// <summary>
/// Repository for user-related operations
/// </summary>
public interface IUserRepository : IRepository<UserDto, Guid>
{
    Task<UserDto?> GetByEmailAsync(string email);
    Task<bool> UpdateBalanceAsync(Guid userId, decimal newBalance, decimal expectedCurrentBalance);
    Task<UserRole> GetUserRoleAsync(Guid userId);
    Task<bool> IsUserExistsByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetUsersByRoleAsync(UserRole role);
    
    // Company relationship methods
    Task<IEnumerable<UserDto>> GetUsersByCompanyIdAsync(Guid companyId);
    Task<bool> AssignUserToCompanyAsync(Guid userId, Guid companyId);
    Task<UserDto?> GetCompanyAdminUserByCompanyIdAsync(Guid companyId);
}