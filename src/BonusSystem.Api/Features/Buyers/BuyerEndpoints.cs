using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;

namespace BonusSystem.Api.Features.Buyers;

public static class BuyerEndpoints
{
    public static WebApplication MapBuyerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/buyers")
            .RequireAuthorization()
            .WithTags("Buyers")
            .WithOpenApi();

        group.MapGet("/context", BuyerHandlers.GetUserContext)
            .WithName("GetBuyerContext")
            .RequireAuthorization();

        group.MapGet("/balance", BuyerHandlers.GetBalance)
            .WithName("GetBuyerBalance")
            .RequireAuthorization();

        group.MapGet("/transactions", BuyerHandlers.GetTransactions)
            .WithName("GetBuyerTransactions")
            .RequireAuthorization();

        group.MapGet("/qrcode", BuyerHandlers.GenerateQrCode)
            .WithName("GenerateBuyerQrCode")
            .RequireAuthorization();

        group.MapPost("/transactions/{id}/cancel", BuyerHandlers.CancelTransaction)
            .WithName("CancelBuyerTransaction")
            .RequireAuthorization();

        group.MapGet("/stores", BuyerHandlers.FindStores)
            .WithName("FindStoresForBuyer")
            .RequireAuthorization();
            
        return app;
    }
}
