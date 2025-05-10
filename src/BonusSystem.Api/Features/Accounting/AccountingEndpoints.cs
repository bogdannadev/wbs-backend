using Microsoft.AspNetCore.Mvc;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Api.Infrastructure.Swagger;

namespace BonusSystem.Api.Features.Accounting;

public static class AccountingEndpoints
{
    public static WebApplication MapAccountingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/accounting")
            .RequireAuthorization()
            .WithTags("Accounting")
            .WithOpenApi();

        // Получение общей статистики
        group.MapGet("/statistics", AccountingHandlers.GetStatistics)
            .WithName("GetAccountingStatistics")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get Accounting Statistics";
                operation.Description = "Retrieves comprehensive statistics for accounting purposes with filtering options\n\n" +
                    "Query parameters:\n" +
                    "- startDate: Start date for the report period\n" +
                    "- endDate: End date for the report period\n" +
                    "- format: Export format (csv/excel/json)";
                
                operation.EnsureResponse("200", "Returns accounting statistics");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        // Получение отчета по транзакциям
        group.MapGet("/transactions/report", AccountingHandlers.GetTransactionsReport)
            .WithName("GetTransactionsReport")
            .RequireAuthorization()
            .WithOpenApi(operation =>
            {
                operation.Summary = "Get Transactions Report";
                operation.Description = "Generates a detailed transactions report for accounting";
                
                operation.EnsureResponse("200", "Returns transactions report");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error"); 
                return operation; 
            });

        // Получение отчета по компаниям
        group.MapGet("/companies/report", AccountingHandlers.GetCompaniesReport)
            .WithName("GetCompaniesReport")
            .RequireAuthorization();

        return app;
    }
}