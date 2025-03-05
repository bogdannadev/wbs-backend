using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace BonusSystem.Api.Tests.Controllers;

public class AdminBffControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IAdminBffService> _adminBffServiceMock;
    private readonly string _validToken = "valid-admin-token";
    private readonly Guid _testAdminId = Guid.NewGuid();

    public AdminBffControllerTests(WebApplicationFactory<Program> factory)
    {
        _adminBffServiceMock = new Mock<IAdminBffService>();
        Mock<IAuthenticationService> authServiceMock = new Mock<IAuthenticationService>();

        // Setup auth service to validate our test token
        authServiceMock
            .Setup(s => s.ValidateTokenAsync(_validToken))
            .ReturnsAsync(true);
        
        authServiceMock
            .Setup(s => s.GetUserIdFromTokenAsync(_validToken))
            .ReturnsAsync(_testAdminId);
        
        authServiceMock
            .Setup(s => s.GetUserRoleAsync(_testAdminId))
            .ReturnsAsync(UserRole.SystemAdmin);
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing services
                var adminServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAdminBffService));
                var authServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthenticationService));

                if (adminServiceDescriptor != null)
                {
                    services.Remove(adminServiceDescriptor);
                }
                
                if (authServiceDescriptor != null)
                {
                    services.Remove(authServiceDescriptor);
                }

                // Add the mock services
                services.AddSingleton(_adminBffServiceMock.Object);
                services.AddSingleton(authServiceMock.Object);
            });
        });
    }

    [Fact]
    public async Task RegisterCompany_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var companyRequest = new CompanyRegistrationDto
        {
            Name = "Test Company",
            ContactEmail = "contact@testcompany.com",
            ContactPhone = "1234567890",
            InitialBonusBalance = 10000m
        };
        
        var companyId = Guid.NewGuid();
        var companyResult = new CompanyRegistrationResultDto
        {
            Success = true,
            Company = new CompanyDto
            {
                Id = companyId,
                Name = companyRequest.Name,
                ContactEmail = companyRequest.ContactEmail,
                ContactPhone = companyRequest.ContactPhone,
                BonusBalance = companyRequest.InitialBonusBalance,
                OriginalBonusBalance = companyRequest.InitialBonusBalance,
                Status = CompanyStatus.Active,
                CreatedAt = DateTime.UtcNow
            }
        };
        
        _adminBffServiceMock
            .Setup(s => s.RegisterCompanyAsync(It.IsAny<CompanyRegistrationDto>()))
            .ReturnsAsync(companyResult);
        
        // Act
        var response = await client.PostAsJsonAsync("/api/admin/companies", companyRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CompanyRegistrationResultDto>();
        
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Company.Should().NotBeNull();
        result.Company!.Name.Should().Be("Test Company");
        result.Company.BonusBalance.Should().Be(10000m);
    }

    [Fact]
    public async Task UpdateCompanyStatus_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var companyId = Guid.NewGuid();
        var status = CompanyStatus.Suspended;
        
        _adminBffServiceMock
            .Setup(s => s.UpdateCompanyStatusAsync(companyId, status))
            .ReturnsAsync(true);
        
        // Act
        var response = await client.PutAsync($"/api/admin/companies/{companyId}/status/{status}", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateCompanyStatus_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var companyId = Guid.NewGuid();
        var status = CompanyStatus.Suspended;
        
        _adminBffServiceMock
            .Setup(s => s.UpdateCompanyStatusAsync(companyId, status))
            .ReturnsAsync(false);
        
        // Act
        var response = await client.PutAsync($"/api/admin/companies/{companyId}/status/{status}", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ModerateStore_ApproveValidStore_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var storeId = Guid.NewGuid();
        bool isApproved = true;
        
        _adminBffServiceMock
            .Setup(s => s.ModerateStoreAsync(storeId, isApproved))
            .ReturnsAsync(true);
        
        // Act
        var response = await client.PutAsync($"/api/admin/stores/{storeId}/moderate?approve={isApproved}", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreditCompanyBalance_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var companyId = Guid.NewGuid();
        var amount = 5000m;
        
        _adminBffServiceMock
            .Setup(s => s.CreditCompanyBalanceAsync(companyId, amount))
            .ReturnsAsync(true);
        
        // Act
        var response = await client.PutAsync($"/api/admin/companies/{companyId}/credit?amount={amount}", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetSystemTransactions_ReturnsTransactions()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var companyId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        
        var expectedTransactions = new List<TransactionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                Amount = 1000m,
                Type = TransactionType.AdminAdjustment,
                Timestamp = DateTime.UtcNow.AddDays(-20),
                Status = TransactionStatus.Completed,
                Description = "Credit balance"
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CompanyId = companyId,
                StoreId = Guid.NewGuid(),
                Amount = 50m,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-15),
                Status = TransactionStatus.Completed
            }
        };
        
        _adminBffServiceMock
            .Setup(s => s.GetSystemTransactionsAsync(companyId, startDate, endDate))
            .ReturnsAsync(expectedTransactions);
        
        // Act
        var response = await client.GetAsync($"/api/admin/transactions?companyId={companyId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactions = await response.Content.ReadFromJsonAsync<List<TransactionDto>>();
        
        transactions.Should().NotBeNull();
        transactions.Should().HaveCount(2);
        transactions![0].Type.Should().Be(TransactionType.AdminAdjustment);
        transactions[1].Type.Should().Be(TransactionType.Earn);
    }

    [Fact]
    public async Task SendSystemNotification_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var recipientId = Guid.NewGuid();
        var message = "System maintenance scheduled for tonight 10 PM UTC";
        var notificationType = NotificationType.System;
        
        _adminBffServiceMock
            .Setup(s => s.SendSystemNotificationAsync(recipientId, message, notificationType))
            .ReturnsAsync(true);
        
        // Setup the request body
        var request = new
        {
            RecipientId = recipientId,
            Message = message,
            Type = notificationType
        };
        
        // Act
        var response = await client.PostAsJsonAsync("/api/admin/notifications", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SendBroadcastNotification_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var message = "Bonus promotion for all users: earn 2x bonus points this weekend!";
        var notificationType = NotificationType.AdminMessage;
        
        _adminBffServiceMock
            .Setup(s => s.SendSystemNotificationAsync(null, message, notificationType))
            .ReturnsAsync(true);
        
        // Setup the request body
        var request = new
        {
            Message = message,
            Type = notificationType
        };
        
        // Act
        var response = await client.PostAsJsonAsync("/api/admin/notifications/broadcast", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}