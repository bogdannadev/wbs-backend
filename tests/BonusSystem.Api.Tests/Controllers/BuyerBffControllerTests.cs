using System.Net;
using System.Net.Http.Headers;
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

public class BuyerBffControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IBuyerBffService> _buyerBffServiceMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly string _validToken = "valid-test-token";
    private readonly Guid _testUserId = Guid.NewGuid();

    public BuyerBffControllerTests(WebApplicationFactory<Program> factory)
    {
        _buyerBffServiceMock = new Mock<IBuyerBffService>();
        _authServiceMock = new Mock<IAuthenticationService>();
        
        // Setup auth service to validate our test token
        _authServiceMock
            .Setup(s => s.ValidateTokenAsync(_validToken))
            .ReturnsAsync(true);
        
        _authServiceMock
            .Setup(s => s.GetUserIdFromTokenAsync(_validToken))
            .ReturnsAsync(_testUserId);
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing services
                var buyerServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IBuyerBffService));
                var authServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthenticationService));

                if (buyerServiceDescriptor != null)
                {
                    services.Remove(buyerServiceDescriptor);
                }
                
                if (authServiceDescriptor != null)
                {
                    services.Remove(authServiceDescriptor);
                }

                // Add the mock services
                services.AddSingleton(_buyerBffServiceMock.Object);
                services.AddSingleton(_authServiceMock.Object);
            });
        });
    }

    [Fact]
    public async Task GetBonusSummary_AuthenticatedUser_ReturnsSummary()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var expectedSummary = new BonusTransactionSummaryDto
        {
            TotalEarned = 500m,
            TotalSpent = 200m,
            CurrentBalance = 300m,
            ExpiringNextQuarter = 50m,
            RecentTransactions = new List<TransactionDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Amount = 100m,
                    Type = TransactionType.Earn,
                    Timestamp = DateTime.UtcNow.AddDays(-1),
                    Status = TransactionStatus.Completed
                }
            }
        };
        
        _buyerBffServiceMock
            .Setup(s => s.GetBonusSummaryAsync(_testUserId))
            .ReturnsAsync(expectedSummary);
        
        // Act
        var response = await client.GetAsync("/api/buyer/bonus-summary");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<BonusTransactionSummaryDto>();
        
        summary.Should().NotBeNull();
        summary!.CurrentBalance.Should().Be(300m);
        summary.TotalEarned.Should().Be(500m);
        summary.TotalSpent.Should().Be(200m);
        summary.ExpiringNextQuarter.Should().Be(50m);
        summary.RecentTransactions.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetBonusSummary_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        // No authorization header
        
        // Act
        var response = await client.GetAsync("/api/buyer/bonus-summary");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTransactionHistory_AuthenticatedUser_ReturnsTransactions()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var expectedTransactions = new List<TransactionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                Amount = 100m,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-1),
                Status = TransactionStatus.Completed
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = _testUserId,
                Amount = 50m,
                Type = TransactionType.Spend,
                Timestamp = DateTime.UtcNow.AddDays(-2),
                Status = TransactionStatus.Completed
            }
        };
        
        _buyerBffServiceMock
            .Setup(s => s.GetTransactionHistoryAsync(_testUserId))
            .ReturnsAsync(expectedTransactions);
        
        // Act
        var response = await client.GetAsync("/api/buyer/transactions");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactions = await response.Content.ReadFromJsonAsync<List<TransactionDto>>();
        
        transactions.Should().NotBeNull();
        transactions.Should().HaveCount(2);
        transactions![0].Amount.Should().Be(100m);
        transactions[1].Amount.Should().Be(50m);
    }

    [Fact]
    public async Task CancelTransaction_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var transactionId = Guid.NewGuid();
        
        _buyerBffServiceMock
            .Setup(s => s.CancelTransactionAsync(_testUserId, transactionId))
            .ReturnsAsync(true);
        
        // Act
        var response = await client.DeleteAsync($"/api/buyer/transactions/{transactionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CancelTransaction_InvalidTransaction_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var transactionId = Guid.NewGuid();
        
        _buyerBffServiceMock
            .Setup(s => s.CancelTransactionAsync(_testUserId, transactionId))
            .ReturnsAsync(false);
        
        // Act
        var response = await client.DeleteAsync($"/api/buyer/transactions/{transactionId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task FindStoresByCategory_ReturnsStores()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var category = "Electronics";
        
        var expectedStores = new List<StoreDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Name = "Electronics Store 1",
                Location = "Location 1",
                Address = "Address 1",
                ContactPhone = "123456789",
                Status = StoreStatus.Active
            },
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Name = "Electronics Store 2",
                Location = "Location 2",
                Address = "Address 2",
                ContactPhone = "987654321",
                Status = StoreStatus.Active
            }
        };
        
        _buyerBffServiceMock
            .Setup(s => s.FindStoresByCategoryAsync(category))
            .ReturnsAsync(expectedStores);
        
        // Act
        var response = await client.GetAsync($"/api/buyer/stores?category={category}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var stores = await response.Content.ReadFromJsonAsync<List<StoreDto>>();
        
        stores.Should().NotBeNull();
        stores.Should().HaveCount(2);
        stores![0].Name.Should().Be("Electronics Store 1");
        stores[1].Name.Should().Be("Electronics Store 2");
    }

    [Fact]
    public async Task GenerateQrCode_AuthenticatedUser_ReturnsQrCodeData()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var expectedQrCode = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==";
        
        _buyerBffServiceMock
            .Setup(s => s.GenerateQrCodeAsync(_testUserId))
            .ReturnsAsync(expectedQrCode);
        
        // Act
        var response = await client.GetAsync("/api/buyer/qr-code");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var qrCode = await response.Content.ReadAsStringAsync();
        
        qrCode.Should().NotBeNullOrEmpty();
        qrCode.Should().Be(expectedQrCode);
    }
}
