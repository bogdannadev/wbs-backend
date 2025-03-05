using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;

namespace BonusSystem.Api.Features.Admin;

public static class AdminEndpoints
{
    public static WebApplication MapAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/admin")
            .RequireAuthorization()
            .WithTags("Admin")
            .WithOpenApi();

        group.MapGet("/context", AdminHandlers.GetUserContext)
            .WithName("GetAdminContext")
            .RequireAuthorization();

        group.MapPost("/companies", AdminHandlers.RegisterCompany)
            .WithName("RegisterCompany")
            .RequireAuthorization();

        group.MapPut("/companies/{id}/status", AdminHandlers.UpdateCompanyStatus)
            .WithName("UpdateCompanyStatus")
            .RequireAuthorization();

        group.MapPut("/stores/{id}/moderate", AdminHandlers.ModerateStore)
            .WithName("ModerateStore")
            .RequireAuthorization();

        group.MapPost("/companies/{id}/credit", AdminHandlers.CreditCompanyBalance)
            .WithName("CreditCompanyBalance")
            .RequireAuthorization();

        group.MapGet("/transactions", AdminHandlers.GetSystemTransactions)
            .WithName("GetSystemTransactions")
            .RequireAuthorization();

        group.MapPost("/notifications", AdminHandlers.SendSystemNotification)
            .WithName("SendSystemNotification")
            .RequireAuthorization();
            
        return app;
    }
}
