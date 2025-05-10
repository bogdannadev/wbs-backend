using BonusSystem.Api.Helpers;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using BonusSystem.Shared.Dtos; 

namespace BonusSystem.Api.Features.Buyers;

public static class BuyerHandlers
{
    public static async Task<IResult> GetUserContext(HttpContext httpContext, IBuyerBffService buyerService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId =>
        {
            var context = await buyerService.GetUserContextAsync(userId);
            var actions = await buyerService.GetPermittedActionsAsync(userId);

            return new { context, actions };
        }, "Error getting user context");
    }
    public static async Task<IResult> GetBalance(HttpContext httpContext, IBuyerBffService buyerService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext,
            async userId => { return await buyerService.GetBonusSummaryAsync(userId); }, "Error getting balance");
    }

    public static async Task<IResult> GetTransactions(HttpContext httpContext, IBuyerBffService buyerService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext,
            async userId => { return await buyerService.GetTransactionHistoryAsync(userId); },
            "Error getting transactions");
    }

    public static async Task<IResult> GenerateQrCode(HttpContext httpContext, IBuyerBffService buyerService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId =>
        {
            var qrCode = await buyerService.GenerateQrCodeAsync(userId);
            return new { qrCode };
        }, "Error generating QR code");
    }

    public static async Task<IResult> CancelTransaction(
        HttpContext httpContext,
        Guid id,
        IBuyerBffService buyerService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await buyerService.CancelTransactionAsync(userId.Value, id);
            if (!success)
            {
                return RequestHelper.CreateErrorResponse("Transaction could not be cancelled");
            }

            return RequestHelper.CreateSuccessResponse("Transaction cancelled successfully");
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error cancelling transaction");
        }
    }

    public static async Task<IResult> FindStores(
        HttpContext httpContext,
        [FromQuery] string category,
        IBuyerBffService buyerService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext,
            async userId => await buyerService.FindStoresByCategoryAsync(category), "Error finding stores");
    }
}