using Microsoft.AspNetCore.Mvc;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Dtos;
using BonusSystem.Api.Helpers;

namespace BonusSystem.Api.Features.Accounting;

public static class AccountingHandlers
{
    public static async Task<IResult> GetStatistics(
        HttpContext httpContext,
        IObserverBffService observerService,
        IStatisticsExportService exportService,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string format = "json")
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId =>
        {
            var query = new StatisticsQueryDto
            {
                StartDate = startDate,
                EndDate = endDate
            };

            var statistics = await observerService.GetStatisticsAsync(query);
            
            switch (format.ToLower())
            {
                case "csv":
                    var csvStream = await exportService.ExportToCsvAsync(statistics);
                    return Results.File(csvStream, "text/csv", "accounting_statistics.csv");
                
                // case "excel":
                //     var excelStream = await exportService.ExportToExcelAsync(statistics);
                //     return Results.File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                //         "accounting_statistics.xlsx");
                
                default:
                    return Results.Ok(statistics);
            }
        }, "Error getting accounting statistics");
    }

    public static async Task<IResult> GetTransactionsReport(
        HttpContext httpContext,
        IAdminBffService adminService,
        IStatisticsExportService exportService,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string format = "json")
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId =>
        {
            var transactions = await adminService.GetSystemTransactionsAsync(null, startDate, endDate);
            
            switch (format.ToLower())
            {
                case "csv":
                    var stream = await exportService.ExportToCsvAsync(transactions);
                    // Reset stream position to beginning
                    stream.Position = 0;
                    return Results.File(
                        fileStream: stream,
                        contentType: "text/csv",
                        fileDownloadName: "transactions_report.csv",
                        enableRangeProcessing: false
                    );
                
                // case "excel":
                //     var excelStream = await exportService.ExportToExcelAsync(transactions);
                //     return Results.File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                //         "transactions_report.xlsx");
                
                default:
                    return Results.Ok(transactions);
            }
        }, "Error getting transactions report");
    }

    public static async Task<IResult> GetCompaniesReport(
        HttpContext httpContext,
        IObserverBffService observerService,
        IStatisticsExportService exportService,
        [FromQuery] string format = "json")
    {
        return await RequestHelper.ProcessAuthenticatedRequest(httpContext, async userId =>
        {
            var companies = await observerService.GetCompaniesOverviewAsync();
            
            switch (format.ToLower())
            {
                case "csv":
                    var stream = await exportService.ExportToCsvAsync(companies);
                    // Reset stream position to beginning
                    stream.Position = 0;
                    return Results.File(
                        fileStream: stream,
                        contentType: "text/csv",
                        fileDownloadName: "companies_report.csv",
                        enableRangeProcessing: false
                    );
                // case "excel":
                //     var excelStream = await exportService.ExportToExcelAsync(companies);
                //     return Results.File(excelStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                //         "companies_report.xlsx");
                
                default:
                    return Results.Ok(companies);
            }
        }, "Error getting companies report");
    }
}