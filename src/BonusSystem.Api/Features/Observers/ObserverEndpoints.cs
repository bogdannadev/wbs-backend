using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;

namespace BonusSystem.Api.Features.Observers;

public static class ObserverEndpoints
{
    public static WebApplication MapObserverEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/observers")
            .RequireAuthorization()
            .WithTags("Observers")
            .WithOpenApi();

        group.MapGet("/context", ObserverHandlers.GetUserContext)
            .WithName("GetObserverContext")
            .RequireAuthorization();

        group.MapGet("/statistics", ObserverHandlers.GetStatistics)
            .WithName("GetObserverStatistics")
            .RequireAuthorization();

        group.MapGet("/transactions/summary", ObserverHandlers.GetTransactionSummary)
            .WithName("GetObserverTransactionSummary")
            .RequireAuthorization();

        group.MapGet("/companies", ObserverHandlers.GetCompaniesOverview)
            .WithName("GetObserverCompaniesOverview")
            .RequireAuthorization();
            
        return app;
    }
}
