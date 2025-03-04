using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BonusSystem.Core.Repositories;
using BonusSystem.Infrastructure.Auth;
using BonusSystem.Infrastructure.DataAccess;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BonusSystem.Infrastructure.Tests.Auth;

public class JwtAuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOptions<AppDbOptions>> _optionsMock;
    private readonly Mock<ILogger<JwtAuthenticationService>> _loggerMock;
    private readonly JwtAuthenticationService _authService;
    private readonly AppDbOptions _appDbOptions;

    public JwtAuthenticationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<JwtAuthenticationService>>();
        
        _appDbOptions = new AppDbOptions
        {
            JwtSecret = "TestSecretKey12345678901234567890",
            TokenExpirationMinutes = 60
        };
        
        _optionsMock = new Mock<IOptions<AppDbOptions>>();
        _optionsMock.Setup(x => x.Value).Returns(_appDbOptions);
        
        _authService = new JwtAuthenticationService(
            _userRepositoryMock.Object,
            _optionsMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SignInAsync_ValidCredentials_ReturnsSuccessWithToken()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Email = "test@example.com",
            Password = "password"
        };
        
        var user = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Username = "testuser",
            Role = UserRole.Buyer
        };
        
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);
        
        // Act
        var result = await _authService.SignInAsync(loginDto);
        
        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.UserId.Should().Be(user.Id);
        result.Token.Should().NotBeNullOrEmpty();
        result.Role.Should().Be(UserRole.Buyer);
    }

    [Fact]
    public async Task SignInAsync_InvalidEmail_ReturnsFailure()
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            Email = "nonexistent@example.com",
            Password = "password"
        };
        
        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((UserDto)null);
        
        // Act
        var result = await _authService.SignInAsync(loginDto);
        
        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        result.Token.Should().BeNull();
    }

    [Fact]
    public async Task SignUpAsync_NewUser_ReturnsSuccessWithToken()
    {
        // Arrange
        var registrationDto = new UserRegistrationDto
        {
            Email = "new@example.com",
            Username = "newuser",
            Password = "password",
            Role = UserRole.Buyer
        };
        
        _userRepositoryMock
            .Setup(r => r.IsUserExistsByEmailAsync(registrationDto.Email))
            .ReturnsAsync(false);
        
        _userRepositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<UserDto>()))
            .ReturnsAsync(Guid.NewGuid);
        
        // Act
        var result = await _authService.SignUpAsync(registrationDto);
        
        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.UserId.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Role.Should().Be(UserRole.Buyer);
    }

    [Fact]
    public async Task SignUpAsync_ExistingEmail_ReturnsFailure()
    {
        // Arrange
        var registrationDto = new UserRegistrationDto
        {
            Email = "existing@example.com",
            Username = "existinguser",
            Password = "password",
            Role = UserRole.Buyer
        };
        
        _userRepositoryMock
            .Setup(r => r.IsUserExistsByEmailAsync(registrationDto.Email))
            .ReturnsAsync(true);
        
        // Act
        var result = await _authService.SignUpAsync(registrationDto);
        
        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        result.Token.Should().BeNull();
    }

    [Fact]
    public async Task GenerateTokenAsync_ValidData_ReturnsValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var role = UserRole.Buyer;
        
        // Act
        var token = await _authService.GenerateTokenAsync(userId, role);
        
        // Assert
        token.Should().NotBeNullOrEmpty();
        
        // Verify token content
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.NameIdentifier && c.Value == userId.ToString());
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == role.ToString());
    }

    [Fact]
    public async Task GetUserIdFromTokenAsync_ValidToken_ReturnsUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = await _authService.GenerateTokenAsync(userId, UserRole.Buyer);
        
        // Act
        var result = await _authService.GetUserIdFromTokenAsync(token);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().Be(userId);
    }

    [Fact]
    public async Task GetUserIdFromTokenAsync_InvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid.token.string";
        
        // Act
        var result = await _authService.GetUserIdFromTokenAsync(invalidToken);
        
        // Assert
        result.Should().BeNull();
    }
}
