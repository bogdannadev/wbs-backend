using BonusSystem.Api.Helpers;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;

namespace BonusSystem.Api.Features.Sellers;

public static class SellerHandlers
{
    public static async Task<IResult> GetUserContext(HttpContext httpContext, ISellerBffService sellerService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            var context = await sellerService.GetUserContextAsync(userId);
            var actions = await sellerService.GetPermittedActionsAsync(userId);
            
            return new { context, actions };
        }, "Error getting user context");
    }

    public static async Task<IResult> ProcessTransaction(
        HttpContext httpContext,
        TransactionRequestDto request,
        ISellerBffService sellerService, 
        decimal cashbackpercent)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var result = await sellerService.ProcessTransactionAsync(userId.Value, request, cashbackpercent);
            if (!result.Success)
            {
                return RequestHelper.CreateErrorResponse(result.ErrorMessage);
            }
            
            return RequestHelper.CreateSuccessResponse(result);
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error processing transaction");
        }
    }

    public static async Task<IResult> ConfirmTransactionReturn(
        HttpContext httpContext,
        Guid id,
        ISellerBffService sellerService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await sellerService.ConfirmTransactionReturnAsync(userId.Value, id);
            if (!success)
            {
                return RequestHelper.CreateErrorResponse("Transaction could not be returned");
            }
            
            return RequestHelper.CreateSuccessResponse("Transaction return confirmed successfully");
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error confirming transaction return");
        }
    }

    public static async Task<IResult> GetBuyerBalance(
        HttpContext httpContext,
        Guid id,
        ISellerBffService sellerService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            var balance = await sellerService.GetBuyerBonusBalanceAsync(id);
            return new { balance };
        }, "Error getting buyer balance");
    }

    public static async Task<IResult> GetStoreBalance(
        HttpContext httpContext,
        ISellerBffService sellerService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            var balance = await sellerService.GetStoreBonusBalanceByUserIdAsync(userId);
            return new { balance };
        }, "Error getting store balance");
    }

    public static async Task<IResult> GetStoreTransactions(
        HttpContext httpContext,
        ISellerBffService sellerService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            return await sellerService.GetStoreBonusTransactionsByUserIdAsync(userId);
        }, "Error getting store transactions");
    }
}