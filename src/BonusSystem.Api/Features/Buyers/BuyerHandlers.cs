using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BonusSystem.Api.Features.Buyers;

public static class BuyerHandlers
{
    public static async Task<IResult> GetUserContext(HttpContext httpContext, IBuyerBffService buyerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var context = await buyerService.GetUserContextAsync(userId.Value);
            var actions = await buyerService.GetPermittedActionsAsync(userId.Value);
            
            return Results.Ok(new { context, actions });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting user context: {ex.Message}");
        }
    }

    public static async Task<IResult> GetBalance(HttpContext httpContext, IBuyerBffService buyerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var summary = await buyerService.GetBonusSummaryAsync(userId.Value);
            return Results.Ok(summary);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting balance: {ex.Message}");
        }
    }

    public static async Task<IResult> GetTransactions(HttpContext httpContext, IBuyerBffService buyerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var transactions = await buyerService.GetTransactionHistoryAsync(userId.Value);
            return Results.Ok(transactions);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting transactions: {ex.Message}");
        }
    }

    public static async Task<IResult> GenerateQrCode(HttpContext httpContext, IBuyerBffService buyerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var qrCode = await buyerService.GenerateQrCodeAsync(userId.Value);
            return Results.Ok(new { qrCode });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error generating QR code: {ex.Message}");
        }
    }

    public static async Task<IResult> CancelTransaction(
        HttpContext httpContext, 
        Guid id, 
        IBuyerBffService buyerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await buyerService.CancelTransactionAsync(userId.Value, id);
            if (!success)
            {
                return Results.BadRequest("Transaction could not be cancelled");
            }
            
            return Results.Ok(new { message = "Transaction cancelled successfully" });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error cancelling transaction: {ex.Message}");
        }
    }

    public static async Task<IResult> FindStores(
        HttpContext httpContext, 
        [FromQuery] string category,
        IBuyerBffService buyerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var stores = await buyerService.FindStoresByCategoryAsync(category);
            return Results.Ok(stores);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error finding stores: {ex.Message}");
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

    public static async Task<string> Test_Task_String()
    {
        return await Task.FromResult("string returned successfully!");
    }
}
