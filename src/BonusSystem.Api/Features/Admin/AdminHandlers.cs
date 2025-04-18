using BonusSystem.Api.Helpers;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BonusSystem.Api.Features.Admin;

public static class AdminHandlers
{
    public static async Task<IResult> GetUserContext(HttpContext httpContext, IAdminBffService adminService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            var context = await adminService.GetUserContextAsync(userId);
            var actions = await adminService.GetPermittedActionsAsync(userId);
            
            return new { context, actions };
        }, "Error getting user context");
    }

    public static async Task<IResult> RegisterCompany(
        HttpContext httpContext,
        CompanyRegistrationDto request,
        IAdminBffService adminService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            var result = await adminService.RegisterCompanyAsync(request);
            if (!result.Success)
            {
                throw new InvalidOperationException(result.ErrorMessage);
            }
            
            return result;
        }, "Error registering company");
    }

    public static async Task<IResult> UpdateCompanyStatus(
        HttpContext httpContext,
        Guid id,
        [FromBody] CompanyStatus status,
        IAdminBffService adminService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await adminService.UpdateCompanyStatusAsync(id, status);
            if (!success)
            {
                return RequestHelper.CreateErrorResponse("Company status could not be updated");
            }
            
            return RequestHelper.CreateSuccessResponse("Company status updated successfully");
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error updating company status");
        }
    }

    public static async Task<IResult> ModerateStore(
        HttpContext httpContext,
        Guid id,
        [FromQuery] bool approve,
        IAdminBffService adminService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await adminService.ModerateStoreAsync(id, approve);
            if (!success)
            {
                return RequestHelper.CreateErrorResponse("Store could not be moderated");
            }
            
            return RequestHelper.CreateSuccessResponse($"Store {(approve ? "approved" : "rejected")} successfully");
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error moderating store");
        }
    }

    public static async Task<IResult> CreditCompanyBalance(
        HttpContext httpContext,
        Guid id,
        [FromBody] decimal amount,
        IAdminBffService adminService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await adminService.CreditCompanyBalanceAsync(id, amount);
            if (!success)
            {
                return RequestHelper.CreateErrorResponse("Company balance could not be credited");
            }
            
            return RequestHelper.CreateSuccessResponse("Company balance credited successfully");
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error crediting company balance");
        }
    }

    public static async Task<IResult> GetSystemTransactions(
        HttpContext httpContext,
        [FromQuery] Guid? companyId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        IAdminBffService adminService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            return await adminService.GetSystemTransactionsAsync(companyId, startDate, endDate);
        }, "Error getting system transactions");
    }

    public static async Task<IResult> SendSystemNotification(
        HttpContext httpContext,
        [FromQuery] Guid? recipientId,
        [FromBody] NotificationRequest request,
        IAdminBffService adminService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await adminService.SendSystemNotificationAsync(recipientId, request.Message, request.Type);
            if (!success)
            {
                return RequestHelper.CreateErrorResponse("Notification could not be sent");
            }
            
            return RequestHelper.CreateSuccessResponse("Notification sent successfully");
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error sending notification");
        }
    }

    public static async Task<IResult> GetTransactionFeeReport(
        HttpContext httpContext,
        TransactionFeeRequest request,
        IAdminBffService adminBffService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            return await adminBffService.GetTransactionFeesAsync(request);
        }, "Error at fee calculation query");
    }
}
