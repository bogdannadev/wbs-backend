using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BonusSystem.Api.Features.Admin;

public static class AdminHandlers
{
    public static async Task<IResult> TestAPI_String_return() 
    { 

        try
        {
            return Results.Ok("String returned, successfully!");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting user context: {ex.Message}");
        }
    }
    public static async Task<IResult> GetUserContext(HttpContext httpContext, IAdminBffService adminService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var context = await adminService.GetUserContextAsync(userId.Value);
            var actions = await adminService.GetPermittedActionsAsync(userId.Value);
            
            return Results.Ok(new { context, actions });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting user context: {ex.Message}");
        }
    }

    public static async Task<IResult> RegisterCompany(
        HttpContext httpContext,
        CompanyRegistrationDto request,
        IAdminBffService adminService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var result = await adminService.RegisterCompanyAsync(request);
            if (!result.Success)
            {
                return Results.BadRequest(new { message = result.ErrorMessage });
            }
            
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error registering company: {ex.Message}");
        }
    }

    public static async Task<IResult> UpdateCompanyStatus(
        HttpContext httpContext,
        Guid id,
        [FromBody] CompanyStatus status,
        IAdminBffService adminService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await adminService.UpdateCompanyStatusAsync(id, status);
            if (!success)
            {
                return Results.BadRequest("Company status could not be updated");
            }
            
            return Results.Ok(new { message = "Company status updated successfully" });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error updating company status: {ex.Message}");
        }
    }

    public static async Task<IResult> ModerateStore(
        HttpContext httpContext,
        Guid id,
        [FromQuery] bool approve,
        IAdminBffService adminService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await adminService.ModerateStoreAsync(id, approve);
            if (!success)
            {
                return Results.BadRequest("Store could not be moderated");
            }
            
            return Results.Ok(new { message = $"Store {(approve ? "approved" : "rejected")} successfully" });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error moderating store: {ex.Message}");
        }
    }

    public static async Task<IResult> CreditCompanyBalance(
        HttpContext httpContext,
        Guid id,
        [FromBody] decimal amount,
        IAdminBffService adminService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await adminService.CreditCompanyBalanceAsync(id, amount);
            if (!success)
            {
                return Results.BadRequest("Company balance could not be credited");
            }
            
            return Results.Ok(new { message = "Company balance credited successfully" });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error crediting company balance: {ex.Message}");
        }
    }

    public static async Task<IResult> GetSystemTransactions(
        HttpContext httpContext,
        [FromQuery] Guid? companyId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        IAdminBffService adminService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var transactions = await adminService.GetSystemTransactionsAsync(companyId, startDate, endDate);
            return Results.Ok(transactions);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting system transactions: {ex.Message}");
        }
    }

    public static async Task<IResult> SendSystemNotification(
        HttpContext httpContext,
        [FromQuery] Guid? recipientId,
        [FromBody] NotificationRequest request,
        IAdminBffService adminService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var success = await adminService.SendSystemNotificationAsync(recipientId, request.Message, request.Type);
            if (!success)
            {
                return Results.BadRequest("Notification could not be sent");
            }
            
            return Results.Ok(new { message = "Notification sent successfully" });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error sending notification: {ex.Message}");
        }
    }

    public static async Task<IResult> GetTransactionFeeReport(
        HttpContext httpContext,
        TransactionFeeRequest request,
        IAdminBffService adminBffService)
    {
        var userId = GetUserIdFromContext(httpContext);
        if (userId == null)
        {
                return Results.Unauthorized();
        }

        try
        {
            return Results.Ok(await adminBffService.GetTransactionFeesAsync(request));
        }
        catch (Exception e)
        {
            return Results.Problem($"Error at fee calculation query: {e.Message}");
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

public class NotificationRequest
{
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.System;
}
