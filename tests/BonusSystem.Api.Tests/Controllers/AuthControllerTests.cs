using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BonusSystem.Api.Tests.Controllers;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IAuthenticationService> _authServiceMock;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _authServiceMock = new Mock<IAuthenticationService>();
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing IAuthenticationService
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthenticationService));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add the mock IAuthenticationService
                services.AddSingleton(_authServiceMock.Object);
            });
        });
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registrationDto = new UserRegistrationDto
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "password",
            Role = UserRole.Buyer
        };
        
        var userId = Guid.NewGuid();
        var authResult = new AuthResult
        {
            Success = true,
            UserId = userId,
            Token = "test-token",
            Role = UserRole.Buyer
        };
        
        _authServiceMock
            .Setup(s => s.SignUpAsync(It.IsAny<UserRegistrationDto>()))
            .ReturnsAsync(authResult);
        
        var content = new StringContent(
            JsonSerializer.Serialize(registrationDto),
            Encoding.UTF8,
            "application/json");
        
        // Act
        var response = await client.PostAsync("/auth/register", content);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        responseContent.GetProperty("userId").GetString().Should().Be(userId.ToString());
        responseContent.GetProperty("token").GetString().Should().Be("test-token");
        responseContent.GetProperty("role").GetString().Should().Be(UserRole.Buyer.ToString());
    }

    [Fact]
    public async Task Register_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registrationDto = new UserRegistrationDto
        {
            Email = "existing@example.com",
            Username = "existinguser",
            Password = "password",
            Role = UserRole.Buyer
        };
        
        var authResult = new AuthResult
        {
            Success = false,
            ErrorMessage = "User with this email already exists"
        };
        
        _authServiceMock
            .Setup(s => s.SignUpAsync(It.IsAny<UserRegistrationDto>()))
            .ReturnsAsync(authResult);
        
        var content = new StringContent(
            JsonSerializer.Serialize(registrationDto),
            Encoding.UTF8,
            "application/json");
        
        // Act
        var response = await client.PostAsync("/auth/register", content);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        responseContent.GetProperty("message").GetString().Should().Be("User with this email already exists");
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new UserLoginDto
        {
            Email = "test@example.com",
            Password = "password"
        };
        
        var userId = Guid.NewGuid();
        var authResult = new AuthResult
        {
            Success = true,
            UserId = userId,
            Token = "test-token",
            Role = UserRole.Buyer
        };
        
        _authServiceMock
            .Setup(s => s.SignInAsync(It.IsAny<UserLoginDto>()))
            .ReturnsAsync(authResult);
        
        var content = new StringContent(
            JsonSerializer.Serialize(loginDto),
            Encoding.UTF8,
            "application/json");
        
        // Act
        var response = await client.PostAsync("/auth/login", content);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        responseContent.GetProperty("userId").GetString().Should().Be(userId.ToString());
        responseContent.GetProperty("token").GetString().Should().Be("test-token");
        responseContent.GetProperty("role").GetString().Should().Be(UserRole.Buyer.ToString());
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var loginDto = new UserLoginDto
        {
            Email = "wrong@example.com",
            Password = "wrongpassword"
        };
        
        var authResult = new AuthResult
        {
            Success = false,
            ErrorMessage = "Invalid email or password"
        };
        
        _authServiceMock
            .Setup(s => s.SignInAsync(It.IsAny<UserLoginDto>()))
            .ReturnsAsync(authResult);
        
        var content = new StringContent(
            JsonSerializer.Serialize(loginDto),
            Encoding.UTF8,
            "application/json");
        
        // Act
        var response = await client.PostAsync("/auth/login", content);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        responseContent.GetProperty("message").GetString().Should().Be("Invalid email or password");
    }
}
