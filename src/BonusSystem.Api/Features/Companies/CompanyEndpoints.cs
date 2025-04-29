using BonusSystem.Api.Infrastructure.Swagger;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;
using Microsoft.OpenApi.Models;

namespace BonusSystem.Api.Features.Companies;

public static class CompanyEndpoints
{
    public static WebApplication MapCompanyEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/companies")
            .RequireAuthorization()
            .WithTags("Companies")
            .WithOpenApi();

        group.MapGet("/context", CompanyHandlers.GetUserContext)
            .WithName("GetCompanyContext")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get Company Context";
                operation.Description =
                    "Retrieves the current company user's context information including profile data and permitted actions. This is typically called after login to initialize the company's dashboard.\n\n" +
                    "Successful response contains:\n" +
                    "- context: Company user's profile information (ID, username, role, bonus balance)\n" +
                    "- actions: List of permitted actions for the company";

                operation.EnsureResponse("200", "Returns company context and allowed actions");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");

                return operation;
            });

        group.MapPost("/stores", CompanyHandlers.RegisterStore)
            .WithName("RegisterStore")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Register a New Store";
                operation.Description =
                    "Creates a new store associated with the company. New stores are set to 'PendingApproval' status until approved by an administrator.\n\n" +
                    "Request requires:\n" +
                    "- companyId: ID of the company the store belongs to\n" +
                    "- name: Name of the store\n" +
                    "- location: Geographic location of the store\n" +
                    "- address: Physical address of the store\n" +
                    "- contactPhone: Contact phone number for the store\n" +
                    "- sellerIds: List of seller IDs to assign to this store (optional)\n\n" +
                    "Successful response returns a success confirmation.";

                operation.EnsureResponse("200", "Store registered successfully");
                operation.EnsureResponse("400", "Registration failed");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");

                return operation;
            });

        group.MapPost("/sellers", CompanyHandlers.RegisterSeller)
            .WithName("RegisterSeller")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Register a New Seller";
                operation.Description =
                    "Creates a new seller user associated with the company. Sellers are store employees who can process transactions with buyers.\n\n" +
                    "Request requires:\n" +
                    "- username: Username for the new seller\n" +
                    "- email: Email address for the new seller\n" +
                    "- password: Password for the new seller account\n" +
                    "- role: Must be 'Seller' (1)\n\n" +
                    "Successful response returns a success confirmation.";

                operation.EnsureResponse("200", "Seller registered successfully");
                operation.EnsureResponse("400", "Registration failed");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");

                return operation;
            });

        group.MapGet("/statistics", CompanyHandlers.GetStatistics)
            .WithName("GetCompanyStatistics")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get Company Statistics";
                operation.Description =
                    "Retrieves dashboard statistics for the company, including bonus circulation, current active bonus, transaction count, and store information.\n\n" +
                    "Query parameters:\n" +
                    "- startDate: Optional. Filter statistics from this date\n" +
                    "- endDate: Optional. Filter statistics to this date\n\n" +
                    "Successful response contains statistics data for the company's dashboard.";

                operation.EnsureResponse("200", "Returns company statistics");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");

                return operation;
            });

        group.MapGet("/transactions", CompanyHandlers.GetTransactionSummary)
            .WithName("GetCompanyTransactionSummary")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get Company Transaction Summary";
                operation.Description =
                    "Retrieves a summary of transactions for the company. This includes the most recent transaction details that help the company monitor bonus point activity.\n\n" +
                    "Successful response contains the most recent transaction data or a placeholder if no transactions exist.";

                operation.EnsureResponse("200", "Returns transaction summary");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");

                return operation;
            });

        group.MapPost("/stores-with-sellers", CompanyHandlers.GetStoresWithSellers)
            .WithName("GetStoresWithSellers")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get Company Stores with Attached Sellers";
                operation.Description =
                    "Retrieves a paginated list of stores for the company with their attached sellers. Supports filtering by store status and seller role.\n\n" +
                    "Request body parameters:\n" +
                    "- storeStatus: Optional. Filter stores by status (0: PendingApproval, 1: Active, 2: Inactive, 3: Rejected)\n" +
                    "- sellerRole: Optional. Filter sellers by role (should always be 1: Seller)\n" +
                    "- page: Optional. Page number (default: 1)\n" +
                    "- pageSize: Optional. Number of items per page (default: 10, max: 100)\n\n" +
                    "Successful response contains a paginated list of stores with their attached sellers.";

                operation.EnsureResponse("200", "Returns stores with sellers");
                operation.EnsureResponse("400", "Bad request - invalid filter parameters");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");

                return operation;
            });

        return app;
    }
}