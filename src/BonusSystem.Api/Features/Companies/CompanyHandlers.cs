using BonusSystem.Api.Helpers;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace BonusSystem.Api.Features.Companies;

public static class CompanyHandlers
{
    public static async Task<IResult> GetUserContext(HttpContext httpContext, ICompanyBffService companyService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            var context = await companyService.GetUserContextAsync(userId);
            var actions = await companyService.GetPermittedActionsAsync(userId);
            
            return new { context, actions };
        }, "Error getting user context");
    }

    public static async Task<IResult> RegisterStore(
        HttpContext httpContext,
        StoreRegistrationDto storeDto,
        ICompanyBffService companyService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        if (!RequestHelper.IsCompanyOrAdminUser(httpContext))
        {
            return Results.Forbid();
        }

        try
        {
            var success = await companyService.RegisterStore(storeDto);
            if (!success)
            {
                return RequestHelper.CreateErrorResponse("Store registration failed");
            }
            
            return RequestHelper.CreateSuccessResponse("Store registered successfully");
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error registering store");
        }
    }

    public static async Task<IResult> RegisterSeller(
        HttpContext httpContext,
        UserRegistrationDto sellerDto,
        ICompanyBffService companyService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        if (!RequestHelper.IsCompanyOrAdminUser(httpContext))
        {
            return Results.Forbid();
        }

        try
        {
            var success = await companyService.RegisterSeller(sellerDto);
            if (!success)
            {
                return RequestHelper.CreateErrorResponse("Seller registration failed");
            }
            
            return RequestHelper.CreateSuccessResponse("Seller registered successfully");
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error registering seller");
        }
    }

    public static async Task<IResult> GetStatistics(
        HttpContext httpContext,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        ICompanyBffService companyService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            var companyId = RequestHelper.GetCompanyIdFromUser(httpContext, userId);
            if (companyId == null)
            {
                throw new InvalidOperationException("Company ID not found for user");
            }

            var query = new StatisticsQueryDto
            {
                CompanyId = companyId,
                StartDate = startDate,
                EndDate = endDate
            };

            return await companyService.GetStatisticsAsync(query);
        }, "Error getting company statistics");
    }

    public static async Task<IResult> GetTransactionSummary(
        HttpContext httpContext,
        ICompanyBffService companyService)
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId => 
        {
            var companyId = RequestHelper.GetCompanyIdFromUser(httpContext, userId);
            if (companyId == null)
            {
                throw new InvalidOperationException("Company ID not found for user");
            }

            return await companyService.GetTransactionSummaryAsync(companyId);
        }, "Error getting transaction summary");
    }
}