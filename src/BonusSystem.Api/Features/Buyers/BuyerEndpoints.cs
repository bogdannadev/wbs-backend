using BonusSystem.Api.Infrastructure.Swagger;
using BonusSystem.Core.Services.Interfaces;
using BonusSystem.Shared.Models;
using Microsoft.OpenApi.Models;

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
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Get Buyer Context";
                operation.Description = "Retrieves the current user's context information including profile data and permitted actions. This is typically called after login to initialize the buyer's dashboard.\n\n" +
                    "Successful response contains:\n" +
                    "- context: User profile information (ID, username, role, bonus balance)\n" +
                    "- actions: List of permitted actions for the buyer";
                
                operation.EnsureResponse("200", "Returns user context and allowed actions");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapGet("/balance", BuyerHandlers.GetBalance)
            .WithName("GetBuyerBalance")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Get Buyer's Bonus Balance";
                operation.Description = "Retrieves the current bonus balance and summary statistics for the authenticated buyer, including total earned, spent, and upcoming expiration information.\n\n" +
                    "Successful response contains:\n" +
                    "- totalEarned: Total bonus points earned by the buyer\n" +
                    "- totalSpent: Total bonus points spent by the buyer\n" +
                    "- currentBalance: Current bonus points balance\n" +
                    "- expiringNextQuarter: Bonus points that will expire at the end of the current quarter\n" +
                    "- recentTransactions: List of recent bonus transactions";
                
                operation.EnsureResponse("200", "Returns bonus balance summary");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapGet("/transactions", BuyerHandlers.GetTransactions)
            .WithName("GetBuyerTransactions")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Get Buyer's Transaction History";
                operation.Description = "Retrieves the complete transaction history for the authenticated buyer, including earned and spent bonus points.\n\n" +
                    "Each transaction record contains:\n" +
                    "- id: Unique transaction identifier\n" +
                    "- amount: Bonus points amount\n" +
                    "- type: Transaction type (0=Earn, 1=Spend, 2=Expire, 3=AdminAdjustment)\n" +
                    "- timestamp: Date and time of the transaction\n" +
                    "- status: Transaction status (0=Pending, 1=Completed, 2=Reversed, 3=Failed)\n" +
                    "- description: Optional description of the transaction";
                
                operation.EnsureResponse("200", "Returns list of transactions");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapGet("/qrcode", BuyerHandlers.GenerateQrCode)
            .WithName("GenerateBuyerQrCode")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Generate QR Code for Buyer";
                operation.Description = "Generates a QR code string for the authenticated buyer that can be displayed as an image. This QR code is used for identification at stores when earning or spending bonus points.\n\n" +
                    "Successful response contains:\n" +
                    "- qrCode: String representation of the QR code that can be rendered as an image";
                
                operation.EnsureResponse("200", "Returns QR code");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapPost("/transactions/{id}/cancel", BuyerHandlers.CancelTransaction)
            .WithName("CancelBuyerTransaction")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Cancel a Transaction";
                operation.Description = "Cancels a specific transaction for the authenticated buyer if it's eligible for cancellation. Transactions can only be cancelled within a specific time window after they were created.\n\n" +
                    "Path parameters:\n" +
                    "- id: The unique identifier (GUID) of the transaction to cancel\n\n" +
                    "Successful response contains a confirmation message.";
                
                operation.EnsureResponse("200", "Transaction cancelled successfully");
                operation.EnsureResponse("400", "Transaction could not be cancelled");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

        group.MapGet("/stores", BuyerHandlers.FindStores)
            .WithName("FindStoresForBuyer")
            .RequireAuthorization()
            .WithOpenApi(operation => 
            {
                operation.Summary = "Find Stores by Category";
                operation.Description = "Searches for participating stores based on the provided category. Helps buyers find stores where they can earn or spend bonus points.\n\n" +
                    "Query parameters:\n" +
                    "- category: Category of stores to search for (e.g., 'grocery', 'electronics', 'clothing')\n\n" +
                    "Successful response contains a list of stores with their details (ID, name, location, address, status).";
                
                operation.EnsureResponse("200", "Returns list of stores");
                operation.EnsureResponse("401", "Unauthorized");
                operation.EnsureResponse("500", "Internal server error");
                
                return operation;
            });

            group.MapGet("/test_string", BuyerHandlers.Test_Task_String)
            .WithName("Test_string")
            .AllowAnonymous()
            .WithOpenApi(operation =>
            {
            operation.Summary = "return String returned successfully!";
            operation.Description = "for Test_task";
    
            operation.EnsureResponse("200", "String returned successfully!");
            operation.EnsureResponse("401", "Unauthorized");
            operation.EnsureResponse("500", "Internal server error");
    
            return operation;   
            });
            
        return app;
    }
}
