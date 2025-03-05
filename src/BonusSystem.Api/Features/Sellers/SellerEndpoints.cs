using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;

namespace BonusSystem.Api.Features.Sellers;

public static class SellerEndpoints
{
    public static WebApplication MapSellerEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/sellers")
            .RequireAuthorization()
            .WithTags("Sellers")
            .WithOpenApi();

        group.MapGet("/context", SellerHandlers.GetUserContext)
            .WithName("GetSellerContext")
            .RequireAuthorization();

        group.MapPost("/transactions", SellerHandlers.ProcessTransaction)
            .WithName("ProcessTransaction")
            .RequireAuthorization();

        group.MapPost("/transactions/{id}/return", SellerHandlers.ConfirmTransactionReturn)
            .WithName("ConfirmTransactionReturn")
            .RequireAuthorization();

        group.MapGet("/buyers/{id}/balance", SellerHandlers.GetBuyerBalance)
            .WithName("GetBuyerBalanceForSeller")
            .RequireAuthorization();

        group.MapGet("/stores/{id}/balance", SellerHandlers.GetStoreBalance)
            .WithName("GetStoreBalanceForSeller")
            .RequireAuthorization();

        group.MapGet("/stores/{id}/transactions", SellerHandlers.GetStoreTransactions)
            .WithName("GetStoreTransactionsForSeller")
            .RequireAuthorization();
            
        return app;
    }
}
