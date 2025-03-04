using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace BonusSystem.Core.Tests.Services;

public class BaseBffServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly BaseBffServiceTestImplementation _baseBffService;

    public BaseBffServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _baseBffService = new BaseBffServiceTestImplementation(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetUserContextAsync_ValidUserId_ReturnsUserContext()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto
        {
            Id = userId,
            Username = "testuser",
            Email = "test@example.com",
            Role = UserRole.Buyer,
            BonusBalance = 100
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(userDto);

        // Act
        var result = await _baseBffService.GetUserContextAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.Username.Should().Be("testuser");
        result.Role.Should().Be(UserRole.Buyer);
        result.BonusBalance.Should().Be(100);
    }

    [Fact]
    public async Task GetUserContextAsync_InvalidUserId_ThrowsException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((UserDto)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _baseBffService.GetUserContextAsync(userId));
    }

    [Fact]
    public async Task GetPermittedActionsAsync_BuyerRole_ReturnsBuyerActions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(r => r.GetUserRoleAsync(userId))
            .ReturnsAsync(UserRole.Buyer);

        // Act
        var result = await _baseBffService.GetPermittedActionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
        result.Should().Contain(a => a.ActionName == "ViewBonusBalance");
        result.Should().Contain(a => a.ActionName == "ViewTransactionHistory");
    }

    [Fact]
    public async Task GetPermittedActionsAsync_SellerRole_ReturnsSellerActions()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(r => r.GetUserRoleAsync(userId))
            .ReturnsAsync(UserRole.Seller);

        // Act
        var result = await _baseBffService.GetPermittedActionsAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThan(0);
        result.Should().Contain(a => a.ActionName == "ProcessTransaction");
        result.Should().Contain(a => a.ActionName == "ConfirmReturn");
    }
}

/// <summary>
/// Test implementation of BaseBffService for testing purposes
/// </summary>
public class BaseBffServiceTestImplementation : IBaseBffService
{
    private readonly IUserRepository _userRepository;

    public BaseBffServiceTestImplementation(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserContextDto> GetUserContextAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
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

    public async Task<IEnumerable<PermittedActionDto>> GetPermittedActionsAsync(Guid userId)
    {
        var role = await _userRepository.GetUserRoleAsync(userId);
        
        return role switch
        {
            UserRole.Buyer => new List<PermittedActionDto>
            {
                new() { ActionName = "ViewBonusBalance", Description = "View bonus balance", Endpoint = "/buyer/balance" },
                new() { ActionName = "ViewTransactionHistory", Description = "View transaction history", Endpoint = "/buyer/transactions" },
                new() { ActionName = "SpendBonuses", Description = "Spend bonuses", Endpoint = "/buyer/spend" }
            },
            UserRole.Seller => new List<PermittedActionDto>
            {
                new() { ActionName = "ProcessTransaction", Description = "Process transaction", Endpoint = "/seller/transaction" },
                new() { ActionName = "ConfirmReturn", Description = "Confirm return", Endpoint = "/seller/return" },
                new() { ActionName = "ViewStoreBalance", Description = "View store balance", Endpoint = "/seller/store/balance" }
            },
            UserRole.SystemAdmin => new List<PermittedActionDto>
            {
                new() { ActionName = "ManageUsers", Description = "Manage users", Endpoint = "/admin/users" },
                new() { ActionName = "ManageCompanies", Description = "Manage companies", Endpoint = "/admin/companies" },
                new() { ActionName = "ViewSystemStats", Description = "View system statistics", Endpoint = "/admin/statistics" }
            },
            _ => new List<PermittedActionDto>()
        };
    }
}
