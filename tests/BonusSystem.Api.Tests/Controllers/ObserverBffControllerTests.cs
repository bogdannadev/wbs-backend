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

public class ObserverBffControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IObserverBffService> _observerBffServiceMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly string _validToken = "valid-observer-token";
    private readonly Guid _testObserverId = Guid.NewGuid();

    public ObserverBffControllerTests(WebApplicationFactory<Program> factory)
    {
        _observerBffServiceMock = new Mock<IObserverBffService>();
        _authServiceMock = new Mock<IAuthenticationService>();
        
        // Setup auth service to validate our test token
        _authServiceMock
            .Setup(s => s.ValidateTokenAsync(_validToken))
            .ReturnsAsync(true);
        
        _authServiceMock
            .Setup(s => s.GetUserIdFromTokenAsync(_validToken))
            .ReturnsAsync(_testObserverId);
        
        _authServiceMock
            .Setup(s => s.GetUserRoleAsync(_testObserverId))
            .ReturnsAsync(UserRole.SystemObserver);
        
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing services
                var observerServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IObserverBffService));
                var authServiceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthenticationService));

                if (observerServiceDescriptor != null)
                {
                    services.Remove(observerServiceDescriptor);
                }
                
                if (authServiceDescriptor != null)
                {
                    services.Remove(authServiceDescriptor);
                }

                // Add the mock services
                services.AddSingleton(_observerBffServiceMock.Object);
                services.AddSingleton(_authServiceMock.Object);
            });
        });
    }

    [Fact]
    public async Task GetStatistics_ValidRequest_ReturnsStatistics()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var query = new StatisticsQueryDto
        {
            CompanyId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(-30),
            EndDate = DateTime.UtcNow
        };
        
        var expectedStatistics = new DashboardStatisticsDto
        {
            TotalBonusCirculation = 10000000m,
            CurrentActiveBonus = 5000000m,
            TotalTransactions = 25000,
            ActiveUsers = 5000,
            ActiveCompanies = 50,
            ActiveStores = 200
        };
        
        _observerBffServiceMock
            .Setup(s => s.GetStatisticsAsync(It.IsAny<StatisticsQueryDto>()))
            .ReturnsAsync(expectedStatistics);
        
        // Act
        var response = await client.GetAsync($"/api/observer/statistics?companyId={query.CompanyId}&startDate={query.StartDate:yyyy-MM-dd}&endDate={query.EndDate:yyyy-MM-dd}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var statistics = await response.Content.ReadFromJsonAsync<DashboardStatisticsDto>();
        
        statistics.Should().NotBeNull();
        statistics!.TotalBonusCirculation.Should().Be(10000000m);
        statistics.CurrentActiveBonus.Should().Be(5000000m);
        statistics.TotalTransactions.Should().Be(25000);
        statistics.ActiveUsers.Should().Be(5000);
        statistics.ActiveCompanies.Should().Be(50);
        statistics.ActiveStores.Should().Be(200);
    }

    [Fact]
    public async Task GetTransactionSummary_ReturnsTransactionSummary()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var companyId = Guid.NewGuid();
        
        var expectedSummary = new TransactionDto
        {
            Id = Guid.Empty, // Summary doesn't have a specific transaction ID
            CompanyId = companyId,
            Amount = 750000m, // Total transaction volume
            Type = TransactionType.AdminAdjustment, // Just a placeholder for the summary
            Timestamp = DateTime.UtcNow,
            Status = TransactionStatus.Completed,
            Description = "Transaction Summary"
        };
        
        _observerBffServiceMock
            .Setup(s => s.GetTransactionSummaryAsync(companyId))
            .ReturnsAsync(expectedSummary);
        
        // Act
        var response = await client.GetAsync($"/api/observer/transactions/summary?companyId={companyId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var summary = await response.Content.ReadFromJsonAsync<TransactionDto>();
        
        summary.Should().NotBeNull();
        summary!.CompanyId.Should().Be(companyId);
        summary.Amount.Should().Be(750000m);
    }

    [Fact]
    public async Task GetCompaniesOverview_ReturnsCompanyDetails()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        
        var expectedCompanies = new List<CompanySummaryDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Company 1",
                BonusBalance = 500000m,
                TransactionVolume = 250000m,
                StoreCount = 25,
                Status = CompanyStatus.Active
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Company 2",
                BonusBalance = 750000m,
                TransactionVolume = 300000m,
                StoreCount = 35,
                Status = CompanyStatus.Active
            }
        };
        
        _observerBffServiceMock
            .Setup(s => s.GetCompaniesOverviewAsync())
            .ReturnsAsync(expectedCompanies);
        
        // Act
        var response = await client.GetAsync("/api/observer/companies");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var companies = await response.Content.ReadFromJsonAsync<List<CompanySummaryDto>>();
        
        companies.Should().NotBeNull();
        companies.Should().HaveCount(2);
        companies![0].Name.Should().Be("Test Company 1");
        companies[1].Name.Should().Be("Test Company 2");
        companies[0].BonusBalance.Should().Be(500000m);
        companies[1].BonusBalance.Should().Be(750000m);
    }

    [Fact]
    public async Task GetStatistics_Unauthenticated_ReturnsUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        // No authorization header
        
        // Act
        var response = await client.GetAsync("/api/observer/statistics");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetStatistics_CompanyObserver_ReturnsLimitedStatistics()
    {
        // Arrange
        var client = _factory.CreateClient();
        var companyObserverToken = "company-observer-token";
        var companyObserverId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        
        // Set up auth for company observer
        _authServiceMock
            .Setup(s => s.ValidateTokenAsync(companyObserverToken))
            .ReturnsAsync(true);
        
        _authServiceMock
            .Setup(s => s.GetUserIdFromTokenAsync(companyObserverToken))
            .ReturnsAsync(companyObserverId);
        
        _authServiceMock
            .Setup(s => s.GetUserRoleAsync(companyObserverId))
            .ReturnsAsync(UserRole.CompanyObserver);
        
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", companyObserverToken);
        
        var expectedStatistics = new DashboardStatisticsDto
        {
            // Limited statistics for just one company
            TotalBonusCirculation = 1500000m,
            CurrentActiveBonus = 750000m,
            TotalTransactions = 5000,
            ActiveUsers = 1000,
            ActiveCompanies = 1,
            ActiveStores = 25
        };
        
        _observerBffServiceMock
            .Setup(s => s.GetStatisticsAsync(It.Is<StatisticsQueryDto>(q => q.CompanyId == companyId)))
            .ReturnsAsync(expectedStatistics);
        
        // Act
        var response = await client.GetAsync($"/api/observer/statistics?companyId={companyId}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var statistics = await response.Content.ReadFromJsonAsync<DashboardStatisticsDto>();
        
        statistics.Should().NotBeNull();
        statistics!.TotalBonusCirculation.Should().Be(1500000m);
        statistics.ActiveCompanies.Should().Be(1); // Should only show 1 company for a company observer
    }
}