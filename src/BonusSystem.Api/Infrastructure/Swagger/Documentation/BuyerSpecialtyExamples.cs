using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BonusSystem.Api.Infrastructure.Swagger.Documentation;

/// <summary>
/// Provides example responses for QR code and specialty buyer endpoints
/// </summary>
public class BuyerSpecialtyExamples : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var methodName = context.MethodInfo?.Name;
        
        if (methodName == "GenerateQrCode")
        {
            AddGenerateQrCodeExample(operation);
        }
        else if (methodName == "FindStores" || methodName == "FindStoresForBuyer")
        {
            AddFindStoresExample(operation);
        }
        else if (methodName == "CancelTransaction")
        {
            AddCancelTransactionExample(operation);
        }
    }
    
    private static void AddGenerateQrCodeExample(OpenApiOperation operation)
    {
        if (operation.Responses.TryGetValue("200", out var successResponse))
        {
            var example = new OpenApiObject
            {
                ["qrCode"] = new OpenApiString("data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCAyOTAgMjkwIj48cGF0aCBkPSJNMywzaDE3djE3aC0xN3pNMjcwLDNoMTd2MTdoLTE3ek0zLDI3MGgxN3YxN2gtMTd6TTIwLDIwaDI1MHYyNTBoLTI1MHoiIGZpbGw9ImJsYWNrIj48L3BhdGg+PC9zdmc+"),
                ["userId"] = new OpenApiString("11111111-1111-1111-1111-111111111111"),
                ["username"] = new OpenApiString("buyer1"),
                ["expirationTimestamp"] = new OpenApiDateTime(DateTime.UtcNow.AddMinutes(15))
            };
            
            successResponse.Content["application/json"].Example = example;
        }
    }
    
    private static void AddCancelTransactionExample(OpenApiOperation operation)
    {
        if (operation.Responses.TryGetValue("200", out var successResponse))
        {
            var example = new OpenApiObject
            {
                ["message"] = new OpenApiString("Transaction cancelled successfully"),
                ["transactionId"] = new OpenApiString("88888888-8888-8888-8888-888888888888"),
                ["refundAmount"] = new OpenApiDouble(50)
            };
            
            successResponse.Content["application/json"].Example = example;
        }
        
        if (operation.Responses.TryGetValue("400", out var errorResponse))
        {
            var example = new OpenApiObject
            {
                ["message"] = new OpenApiString("Transaction cannot be cancelled (beyond cancellation window)")
            };
            
            errorResponse.Content["application/json"].Example = example;
        }
    }
    
    private static void AddFindStoresExample(OpenApiOperation operation)
    {
        if (operation.Responses.TryGetValue("200", out var successResponse))
        {
            var example = new OpenApiArray
            {
                new OpenApiObject
                {
                    ["id"] = new OpenApiString("77777777-7777-7777-7777-777777777777"),
                    ["companyId"] = new OpenApiString("66666666-6666-6666-6666-666666666666"),
                    ["name"] = new OpenApiString("Sample Store"),
                    ["location"] = new OpenApiString("Downtown"),
                    ["address"] = new OpenApiString("123 Main St, Anytown, AN 12345"),
                    ["contactPhone"] = new OpenApiString("+1-555-987-6543"),
                    ["status"] = new OpenApiInteger(0) // StoreStatus.Active
                },
                new OpenApiObject
                {
                    ["id"] = new OpenApiString("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    ["companyId"] = new OpenApiString("66666666-6666-6666-6666-666666666666"),
                    ["name"] = new OpenApiString("Electronics Store"),
                    ["location"] = new OpenApiString("Uptown"),
                    ["address"] = new OpenApiString("456 High St, Anytown, AN 12345"),
                    ["contactPhone"] = new OpenApiString("+1-555-456-7890"),
                    ["status"] = new OpenApiInteger(0) // StoreStatus.Active
                }
            };
            
            successResponse.Content["application/json"].Example = example;
        }
    }
}