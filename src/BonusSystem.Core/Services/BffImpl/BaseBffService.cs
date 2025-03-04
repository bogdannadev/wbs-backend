using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using Microsoft.Extensions.Logging;

namespace BonusSystem.Core.Services.BffImpl;

/// <summary>
/// Base implementation of BFF service with common functionality
/// </summary>
public abstract class BaseBffService : IBaseBffService
{
    protected readonly IUserRepository _userRepository;
    protected readonly ILogger<BaseBffService> _logger;

    protected BaseBffService(
        IUserRepository userRepository,
        ILogger<BaseBffService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get user context information for UI rendering and access control
    /// </summary>
    public virtual async Task<UserContextDto> GetUserContextAsync(Guid userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId);
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

            return new UserContextDto
            {
                UserId = user.Id,
                Username = user.Username,
                Role = user.Role,
                BonusBalance = user.BonusBalance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user context for {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Get permitted actions for the current user based on their role
    /// </summary>
    public abstract Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId);

    /// <summary>
    /// Validate user has appropriate role for the operation
    /// </summary>
    protected virtual async Task ValidateUserRoleAsync(Guid userId, params Shared.Models.UserRole[] allowedRoles)
    {
        var userRole = await _userRepository.GetUserRoleAsync(userId);
        
        if (!allowedRoles.Contains(userRole))
        {
            _logger.LogWarning("User {UserId} with role {UserRole} attempted unauthorized access", userId, userRole);
            throw new UnauthorizedAccessException($"User does not have the required role for this operation");
        }
    }
}