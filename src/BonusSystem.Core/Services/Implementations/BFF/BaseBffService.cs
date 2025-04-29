using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Core.Services.Implementations.BFF;

/// <summary>
/// Base implementation for all BFF services
/// </summary>
public abstract class BaseBffService : IBaseBffService
{
    protected readonly IDataService _dataService;
    protected readonly IAuthenticationService _authService;

    protected BaseBffService(
        IDataService dataService,
        IAuthenticationService authService)
    {
        _dataService = dataService;
        _authService = authService;
    }

    /// <summary>
    /// Gets the user context for the specified user
    /// </summary>
    public virtual async Task<UserContextDto> GetUserContextAsync(Guid userId)
    {
        var user = await _dataService.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {userId} not found");
        }

        return new UserContextDto
        {
            UserId = user.Id,
            Username = user.Username,
            Role = user.Role,
            BonusBalance = user.BonusBalance,
            CompanyId = user.CompanyId
        };
    }

    /// <summary>
    /// Gets the permitted actions for the specified user
    /// </summary>
    public virtual async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        // Base implementation returns an empty list
        // Role-specific services should override this method
        return Enumerable.Empty<PermittedActionDto>();
    }
}
