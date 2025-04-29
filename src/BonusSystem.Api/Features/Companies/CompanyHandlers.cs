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
            return Results.Unauthorized();

        if (!RequestHelper.IsCompanyOrAdminUser(httpContext))
            return Results.Forbid();

        try
        {
            var success = await companyService.RegisterStore(storeDto);
            return success
                ? RequestHelper.CreateSuccessResponse("Store registered successfully")
                : RequestHelper.CreateErrorResponse("Store registration failed");
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error registering store");
        }
    }

    public static async Task<IResult> RegisterSeller(
        HttpContext httpContext,
        SellerRegistrationDto sellerDto,
        ICompanyBffService companyService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        
        if (userId == null)
            return Results.Unauthorized();

        if (!RequestHelper.IsCompanyOrAdminUser(httpContext))
            return Results.Forbid();

        try
        {
            // Get company ID from user context
            var companyId = RequestHelper.GetCompanyIdFromUser(httpContext, userId.Value);
            
            if (companyId == null)
                return RequestHelper.CreateErrorResponse("Company ID not found for the current user. User may not be associated with a company.", StatusCodes.Status400BadRequest);
            
            var success = await companyService.RegisterSellerForCompany(sellerDto, companyId.Value);
            
            return success
                ? RequestHelper.CreateSuccessResponse("Seller registered successfully")
                : RequestHelper.CreateErrorResponse("Seller registration failed");
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
                throw new InvalidOperationException("Company ID not found for user");

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
                throw new InvalidOperationException("Company ID not found for user");

            return await companyService.GetTransactionSummaryAsync(companyId);
        }, "Error getting transaction summary");
    }
    
    public static async Task<IResult> GetStoresWithSellers(
        HttpContext httpContext,
        [FromBody] StoresFilterRequestDto filter,
        ICompanyBffService companyService)
    {
        var userId = RequestHelper.GetUserIdFromContext(httpContext);
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        try
        {
            var companyId = RequestHelper.GetCompanyIdFromUser(httpContext, userId.Value);
            if (companyId == null)
            {
                return RequestHelper.CreateErrorResponse("Company ID not found for the current user. User may not be associated with a company.", StatusCodes.Status400BadRequest);
            }
            
            // Apply default values and validate filter
            if (filter == null)
            {
                filter = new StoresFilterRequestDto
                {
                    Page = 1,
                    PageSize = 10,
                    SellerRole = UserRole.Seller
                };
            }
            else
            {
                // Validate enum values
                if (filter.StoreStatus.HasValue && !Enum.IsDefined(typeof(StoreStatus), filter.StoreStatus.Value))
                {
                    return RequestHelper.CreateErrorResponse($"Invalid store status value: {filter.StoreStatus.Value}", StatusCodes.Status400BadRequest);
                }
                
                // Sanitize pagination parameters
                if (filter.Page < 1) filter = filter with { Page = 1 };
                if (filter.PageSize < 1) filter = filter with { PageSize = 10 };
                if (filter.PageSize > 100) filter = filter with { PageSize = 100 };

            }
            
            var result = await companyService.GetStoresWithSellersAsync(companyId.Value, filter);
            return RequestHelper.CreateSuccessResponse(result);
        }
        catch (ArgumentException ex)
        {
            return RequestHelper.CreateErrorResponse(ex.Message, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            return RequestHelper.HandleExceptionResponse(ex, "Error getting stores with sellers");
        }
    }
}