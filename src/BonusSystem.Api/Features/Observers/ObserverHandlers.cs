using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BonusSystem.Api.Features.Observers;

public static class ObserverHandlers
{
    public static async Task<IResult> GetUserContext(HttpContext httpContext, IObserverBffService observerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var context = await observerService.GetUserContextAsync(userId.Value);
            var actions = await observerService.GetPermittedActionsAsync(userId.Value);
            
            return Results.Ok(new { context, actions });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting user context: {ex.Message}");
        }
    }

    public static async Task<IResult> GetStatistics(
        HttpContext httpContext,
        [FromQuery] Guid? companyId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        IObserverBffService observerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var query = new StatisticsQueryDto
            {
                CompanyId = companyId,
                StartDate = startDate,
                EndDate = endDate
            };
            
            var statistics = await observerService.GetStatisticsAsync(query);
            return Results.Ok(statistics);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting statistics: {ex.Message}");
        }
    }

    public static async Task<IResult> GetTransactionSummary(
        HttpContext httpContext,
        [FromQuery] Guid? companyId,
        IObserverBffService observerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var summary = await observerService.GetTransactionSummaryAsync(companyId);
            return Results.Ok(summary);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting transaction summary: {ex.Message}");
        }
    }

    public static async Task<IResult> GetCompaniesOverview(
        HttpContext httpContext,
        IObserverBffService observerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var overview = await observerService.GetCompaniesOverviewAsync();
            return Results.Ok(overview);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting companies overview: {ex.Message}");
        }
    }

    private static Guid? GetUserIdFromContext(HttpContext httpContext)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }
}
