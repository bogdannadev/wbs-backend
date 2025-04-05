using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using System.Security.Claims;

namespace BonusSystem.Api.Features.Sellers;

public static class SellerHandlers
{
    public static async Task<IResult> GetUserContext(HttpContext httpContext, ISellerBffService sellerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var context = await sellerService.GetUserContextAsync(userId.Value);
            var actions = await sellerService.GetPermittedActionsAsync(userId.Value);
            
            return Results.Ok(new { context, actions });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting user context: {ex.Message}");
        }
    }

    public static async Task<IResult> ProcessTransaction(
        HttpContext httpContext,
        TransactionRequestDto request,
        ISellerBffService sellerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var result = await sellerService.ProcessTransactionAsync(userId.Value, request);
            if (!result.Success)
            {
                return Results.BadRequest(new { message = result.ErrorMessage });
            }
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error processing transaction: {ex.Message}");
        }
    }

    public static async Task<IResult> ConfirmTransactionReturn(
        HttpContext httpContext,
        Guid id,
        ISellerBffService sellerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await sellerService.ConfirmTransactionReturnAsync(userId.Value, id);
            if (!success)
            {
                return Results.BadRequest("Transaction could not be returned");
            }
            
            return Results.Ok(new { message = "Transaction return confirmed successfully" });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error confirming transaction return: {ex.Message}");
        }
    }

    public static async Task<IResult> GetBuyerBalance(
        HttpContext httpContext,
        Guid id,
        ISellerBffService sellerService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var balance = await sellerService.GetBuyerBonusBalanceAsync(id);
            return Results.Ok(new { balance });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting buyer balance: {ex.Message}");
        }
    }

    public static async Task<IResult> GetStoreBalance(
        HttpContext httpContext,
        Guid userId,
        ISellerBffService sellerService)
    {
        var userIdFromRequest = GetUserIdFromContext(httpContext);
        if (userIdFromRequest == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var balance = await sellerService.GetStoreBonusBalanceByUserIdAsync(userId);
            return Results.Ok(new { balance });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting store balance: {ex.Message}");
        }
    }

    public static async Task<IResult> GetStoreTransactions(
        HttpContext httpContext,
        Guid userId,
        ISellerBffService sellerService)
    {
        var userIdFromRequest = GetUserIdFromContext(httpContext);
        if (userIdFromRequest == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var transactions = await sellerService.GetStoreBonusTransactionsByUserIdAsync(userId);
            return Results.Ok(transactions);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting store transactions: {ex.Message}");
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
