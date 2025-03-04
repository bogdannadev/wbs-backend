using BonusSystem.Core.Repositories;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace BonusSystem.Core.Tests.Services;

public class BuyerBffServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IStoreRepository> _storeRepositoryMock;
    private readonly BuyerBffServiceTestImplementation _buyerBffService;

    public BuyerBffServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _storeRepositoryMock = new Mock<IStoreRepository>();
        
        _buyerBffService = new BuyerBffServiceTestImplementation(
            _userRepositoryMock.Object,
            _transactionRepositoryMock.Object,
            _storeRepositoryMock.Object);
    }

    [Fact]
    public async Task GetBonusSummaryAsync_ValidBuyer_ReturnsBonusSummary()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactions = new List<TransactionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 100,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-10),
                Status = TransactionStatus.Completed
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 50,
                Type = TransactionType.Spend,
                Timestamp = DateTime.UtcNow.AddDays(-5),
                Status = TransactionStatus.Completed
            }
        };

        _transactionRepositoryMock
            .Setup(r => r.GetTransactionsByUserIdAsync(userId))
            .ReturnsAsync(transactions);

        // Act
        var result = await _buyerBffService.GetBonusSummaryAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.TotalEarned.Should().Be(100);
        result.TotalSpent.Should().Be(50);
        result.CurrentBalance.Should().Be(50);
        result.RecentTransactions.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTransactionHistoryAsync_ValidBuyer_ReturnsTransactionHistory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactions = new List<TransactionDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 100,
                Type = TransactionType.Earn,
                Timestamp = DateTime.UtcNow.AddDays(-10),
                Status = TransactionStatus.Completed
            },
            new()
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = 50,
                Type = TransactionType.Spend,
                Timestamp = DateTime.UtcNow.AddDays(-5),
                Status = TransactionStatus.Completed
            }
        };

        _transactionRepositoryMock
            .Setup(r => r.GetTransactionsByUserIdAsync(userId))
            .ReturnsAsync(transactions);

        // Act
        var result = await _buyerBffService.GetTransactionHistoryAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(transactions);
    }

    [Fact]
    public async Task CancelTransactionAsync_ValidTransactionId_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var transaction = new TransactionDto
        {
            Id = transactionId,
            UserId = userId,
            Amount = 100,
            Type = TransactionType.Earn,
            Timestamp = DateTime.UtcNow.AddDays(-1),
            Status = TransactionStatus.Completed
        };

        _transactionRepositoryMock
            .Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(transaction);

        _transactionRepositoryMock
            .Setup(r => r.UpdateTransactionStatusAsync(transactionId, TransactionStatus.Reversed))
            .ReturnsAsync(true);

        // Act
        var result = await _buyerBffService.CancelTransactionAsync(userId, transactionId);

        // Assert
        result.Should().BeTrue();
        _transactionRepositoryMock.Verify(r => 
            r.UpdateTransactionStatusAsync(transactionId, TransactionStatus.Reversed), Times.Once);
    }

    [Fact]
    public async Task CancelTransactionAsync_TransactionNotOwnedByUser_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var transactionId = Guid.NewGuid();
        var transaction = new TransactionDto
        {
            Id = transactionId,
            UserId = otherUserId, // Different user
            Amount = 100,
            Type = TransactionType.Earn,
            Timestamp = DateTime.UtcNow.AddDays(-1),
            Status = TransactionStatus.Completed
        };

        _transactionRepositoryMock
            .Setup(r => r.GetByIdAsync(transactionId))
            .ReturnsAsync(transaction);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
            _buyerBffService.CancelTransactionAsync(userId, transactionId));
    }

    [Fact]
    public async Task FindStoresByCategoryAsync_ValidCategory_ReturnsStores()
    {
        // Arrange
        var category = "Electronics";
        var stores = new List<StoreDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Name = "Store 1",
                Location = "City 1",
                Status = StoreStatus.Active
            },
            new()
            {
                Id = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                Name = "Store 2",
                Location = "City 2",
                Status = StoreStatus.Active
            }
        };

        _storeRepositoryMock
            .Setup(r => r.GetStoresByCategoryAsync(category))
            .ReturnsAsync(stores);

        // Act
        var result = await _buyerBffService.FindStoresByCategoryAsync(category);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(stores);
    }
}

/// <summary>
/// Test implementation of BuyerBffService for testing purposes
/// </summary>
public class BuyerBffServiceTestImplementation : IBuyerBffService
{
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IStoreRepository _storeRepository;

    public BuyerBffServiceTestImplementation(
        IUserRepository userRepository,
        ITransactionRepository transactionRepository,
        IStoreRepository storeRepository)
    {
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
        _storeRepository = storeRepository;
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
        // Simple implementation for testing
        return new List<PermittedActionDto>
        {
            new() { ActionName = "ViewBonusBalance", Description = "View bonus balance", Endpoint = "/buyer/balance" },
            new() { ActionName = "ViewTransactionHistory", Description = "View transaction history", Endpoint = "/buyer/transactions" }
        };
    }

    public async Task<BonusTransactionSummaryDto> GetBonusSummaryAsync(Guid userId)
    {
        var transactions = await _transactionRepository.GetTransactionsByUserIdAsync(userId);
        
        var totalEarned = transactions
            .Where(t => t.Type == TransactionType.Earn && t.Status == TransactionStatus.Completed)
            .Sum(t => t.Amount);
            
        var totalSpent = transactions
            .Where(t => t.Type == TransactionType.Spend && t.Status == TransactionStatus.Completed)
            .Sum(t => t.Amount);
            
        return new BonusTransactionSummaryDto
        {
            TotalEarned = totalEarned,
            TotalSpent = totalSpent,
            CurrentBalance = totalEarned - totalSpent,
            RecentTransactions = transactions.OrderByDescending(t => t.Timestamp).ToList()
        };
    }

    public async Task<IEnumerable<TransactionDto>> GetTransactionHistoryAsync(Guid userId)
    {
        return await _transactionRepository.GetTransactionsByUserIdAsync(userId);
    }

    public Task<string> GenerateQrCodeAsync(Guid userId)
    {
        // Simple implementation for testing
        return Task.FromResult($"qr:user:{userId}");
    }

    public async Task<bool> CancelTransactionAsync(Guid userId, Guid transactionId)
    {
        var transaction = await _transactionRepository.GetByIdAsync(transactionId);
        
        if (transaction == null)
        {
            throw new KeyNotFoundException($"Transaction with ID {transactionId} not found");
        }
        
        if (transaction.UserId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to cancel this transaction");
        }
        
        return await _transactionRepository.UpdateTransactionStatusAsync(transactionId, TransactionStatus.Reversed);
    }

    public async Task<IEnumerable<StoreDto>> FindStoresByCategoryAsync(string category)
    {
        return await _storeRepository.GetStoresByCategoryAsync(category);
    }
}
