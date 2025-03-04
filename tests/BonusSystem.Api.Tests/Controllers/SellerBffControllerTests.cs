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

public class SellerBffControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<ISellerBffService> _sellerBffServiceMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly string _validToken = "valid-seller-token";
    private readonly Guid _testSellerId = Guid.NewGuid();
    private readonly Guid _testStoreId = Guid.NewGuid();

    public SellerBffControllerTests(WebApplicationFactory<Program> factory)
    {
        _sellerBffServiceMock = new Mock<ISellerBffService>();
        _authServiceMock = new Mock<IAuthenticationService>();
        
        // Setup auth service to validate our test token
        _authServiceMock
            .Setup(s => s.ValidateTokenAsync(_validToken))
            .ReturnsAsync(true);
        
        _authServiceMock
            .Setup(s => s.GetUserIdFromTokenAsync(_validToken))
            .ReturnsAsync(_testSellerId);
        
        _authServiceMock
            .Setup(s => s.GetUserRoleAsync(_testSellerId))
            .ReturnsAsync(UserRole.Seller);
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing services
                var sellerServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ISellerBffService));
                var authServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthenticationService));

                if (sellerServiceDescriptor != null)
                {
                    services.Remove(sellerServiceDescriptor);
                }
                
                if (authServiceDescriptor != null)
                {
                    services.Remove(authServiceDescriptor);
                }

                // Add the mock services
                services.AddSingleton(_sellerBffServiceMock.Object);
                services.AddSingleton(_authServiceMock.Object);
            });
        });
    }

    [Fact]
    public async Task ProcessTransaction_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var transactionRequest = new TransactionRequestDto
        {
            BuyerId = Guid.NewGuid(),
            Amount = 50m,
            Type = TransactionType.Earn,
            StoreId = _testStoreId
        };
        
        var transactionResult = new TransactionResultDto
        {
            Success = true,
            Transaction = new TransactionDto
            {
                Id = Guid.NewGuid(),
                UserId = transactionRequest.BuyerId,
                StoreId = transactionRequest.StoreId,
                Amount = transactionRequest.Amount,
                Type = transactionRequest.Type,
                Status = TransactionStatus.Completed,
                Timestamp = DateTime.UtcNow
            }
        };
        
        _sellerBffServiceMock
            .Setup(s => s.ProcessTransactionAsync(_testSellerId, It.IsAny<TransactionRequestDto>()))
            .ReturnsAsync(transactionResult);
        
        // Act
        var response = await client.PostAsJsonAsync("/api/seller/transactions", transactionRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<TransactionResultDto>();
        
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Transaction.Should().NotBeNull();
        result.Transaction!.Amount.Should().Be(50m);
    }

    [Fact]
    public async Task ProcessTransaction_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var transactionRequest = new TransactionRequestDto
        {
            BuyerId = Guid.NewGuid(),
            Amount = 500m, // Assuming this is too high for single transaction
            Type = TransactionType.Spend,
            StoreId = _testStoreId
        };
        
        var transactionResult = new TransactionResultDto
        {
            Success = false,
            ErrorMessage = "Insufficient bonus balance for the transaction"
        };
        
        _sellerBffServiceMock
            .Setup(s => s.ProcessTransactionAsync(_testSellerId, It.IsAny<TransactionRequestDto>()))
            .ReturnsAsync(transactionResult);
        
        // Act
        var response = await client.PostAsJsonAsync("/api/seller/transactions", transactionRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        
        result.GetProperty("errorMessage").GetString().Should().Be("Insufficient bonus balance for the transaction");
    }

    [Fact]
    public async Task ConfirmTransactionReturn_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var transactionId = Guid.NewGuid();
        
        _sellerBffServiceMock
            .Setup(s => s.ConfirmTransactionReturnAsync(_testSellerId, transactionId))
            .ReturnsAsync(true);
        
        // Act
        var response = await client.PostAsync($"/api/seller/transactions/{transactionId}/return", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ConfirmTransactionReturn_InvalidTransaction_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var transactionId = Guid.NewGuid();
        
        _sellerBffServiceMock
            .Setup(s => s.ConfirmTransactionReturnAsync(_testSellerId, transactionId))
            .ReturnsAsync(false);
        
        // Act
        var response = await client.PostAsync($"/api/seller/transactions/{transactionId}/return", null);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBuyerBonusBalance_ReturnsBalance()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var buyerId = Guid.NewGuid();
        var expectedBalance = 250m;
        
        _sellerBffServiceMock
            .Setup(s => s.GetBuyerBonusBalanceAsync(buyerId))
            .ReturnsAsync(expectedBalance);
        
        // Act
        var response = await client.GetAsync($"/api/seller/buyers/{buyerId}/balance");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var balance = await response.Content.ReadFromJsonAsync<decimal>();
        
        balance.Should().Be(expectedBalance);
    }

    [Fact]
    public async Task GetStoreBonusBalance_ReturnsBalance()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var expectedBalance = 5000m;
        
        _sellerBffServiceMock
            .Setup(s => s.GetStoreBonusBalanceAsync(_testStoreId))
            .ReturnsAsync(expectedBalance);
        
        // Act
        var response = await client.GetAsync($"/api/seller/stores/{_testStoreId}/balance");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var balance = await response.Content.ReadFromJsonAsync<decimal>();
        
        balance.Should().Be(expectedBalance);
    }

    [Fact]
    public async Task GetStoreTransactions_ReturnsTransactions()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var expectedTransactions = new List<StoreBonusTransactionsDto>
        {
            new()
            {
                StoreId = _testStoreId,
                StoreName = "Test Store",
                TotalTransactions = 150m,
                Transactions = new List<TransactionDto>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        StoreId = _testStoreId,
                        Amount = 100m,
                        Type = TransactionType.Earn,
                        Timestamp = DateTime.UtcNow.AddDays(-1),
                        Status = TransactionStatus.Completed
                    },
                    new()
                    {
                        Id = Guid.NewGuid(),
                        UserId = Guid.NewGuid(),
                        StoreId = _testStoreId,
                        Amount = 50m,
                        Type = TransactionType.Spend,
                        Timestamp = DateTime.UtcNow.AddDays(-2),
                        Status = TransactionStatus.Completed
                    }
                }
            }
        };
        
        _sellerBffServiceMock
            .Setup(s => s.GetStoreBonusTransactionsAsync(_testStoreId))
            .ReturnsAsync(expectedTransactions);
        
        // Act
        var response = await client.GetAsync($"/api/seller/stores/{_testStoreId}/transactions");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var transactions = await response.Content.ReadFromJsonAsync<List<StoreBonusTransactionsDto>>();
        
        transactions.Should().NotBeNull();
        transactions.Should().HaveCount(1);
        transactions![0].StoreId.Should().Be(_testStoreId);
        transactions[0].Transactions.Should().HaveCount(2);
        transactions[0].TotalTransactions.Should().Be(150m);
    }
}
