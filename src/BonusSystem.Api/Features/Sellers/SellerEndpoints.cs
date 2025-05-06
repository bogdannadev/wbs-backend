using BonusSystem.Api.Infrastructure.Swagger;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;
using Microsoft.OpenApi.Models;

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
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Get Seller Context";
                operation.Description = "Retrieves the seller's user context and permitted actions. This is typically called after login to initialize the seller's interface.\n\n" +
                    "Successful response contains:\n" +
                    "- context: Seller's user context information (ID, username, role)\n" +
                    "- actions: List of actions permitted for the seller";
                
                operation.EnsureResponse("200", "Returns seller context and actions");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapPost("/transactions/process", SellerHandlers.ProcessTransaction)
            .WithName("ProcessTransaction")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Process a Transaction";
                operation.Description = "Creates a new bonus transaction for a buyer at the seller's store. Can be used for both earning (adding) and spending (subtracting) bonus points during a purchase.\n\n" +
                    "Request requires:\n" +
                    "- buyerId: Unique identifier of the buyer (from QR code scan)\n" +
                    "- amount: Bonus amount for the transaction (positive value)\n" +
                    "- type: Transaction type (0=Earn, 1=Spend)\n" + 
                    "- cashbackPercent: Percentage of cashback to be applied (0-100)\n\n" +
                    "Successful response contains the processed transaction details.";
                
                operation.EnsureResponse("200", "Transaction processed successfully");
                operation.EnsureResponse("400", "Transaction failed");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapPost("/transactions/{id}/return", SellerHandlers.ConfirmTransactionReturn)
            .WithName("ConfirmTransactionReturn")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Confirm Transaction Return";
                operation.Description = "Confirms a transaction return/cancellation that was initiated by a buyer. The seller must verify and approve the return request.\n\n" +
                    "Path parameters:\n" +
                    "- id: The unique identifier (GUID) of the transaction to confirm return for\n\n" +
                    "Successful response contains a confirmation message.";
                
                operation.EnsureResponse("200", "Transaction return confirmed");
                operation.EnsureResponse("400", "Transaction could not be returned");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapGet("/buyers/{Id}/balance", SellerHandlers.GetBuyerBalance)
            .WithName("GetBuyerBalanceForSeller")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Get Buyer's Balance";
                operation.Description = "Retrieves the current bonus balance for a specific buyer. Allows sellers to verify if a buyer has sufficient bonus points for a transaction.\n\n" +
                    "Path parameters:\n" +
                    "- id: The unique identifier (GUID) of the buyer\n\n" +
                    "Successful response contains the current bonus balance for the specified buyer.";
                
                operation.EnsureResponse("200", "Returns buyer's balance");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapGet("/balance", SellerHandlers.GetStoreBalance)
            .WithName("GetStoreBalanceForSeller")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Get Store's Balance";
                operation.Description = "Retrieves the current bonus balance for a specific store. Shows how many bonus points the store has available for distribution to buyers.\n\n" +
                    "Path parameters:\n" +
                    "- id: The unique identifier (GUID) of the seller's user id\n\n" +
                    "Successful response contains the current bonus balance for the specified store.";
                
                operation.EnsureResponse("200", "Returns store's balance");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapGet("/transactions/list", SellerHandlers.GetStoreTransactions)
            .WithName("GetStoreTransactionsForSeller")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Get Store's Transactions";
                operation.Description = "Retrieves the transaction history for a specific store. Shows all bonus point transactions that occurred at the store.\n\n" +
                    "Path parameters:\n" +
                    "- id: The unique identifier (GUID) of the seller's user id\n\n" +
                    "Successful response contains a list of transactions with their details.";
                
                operation.EnsureResponse("200", "Returns store's transactions");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });
            
        return app;
    }
}
