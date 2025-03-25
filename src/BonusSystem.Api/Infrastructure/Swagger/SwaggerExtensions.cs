using Microsoft.OpenApi.Models;

namespace BonusSystem.Api.Infrastructure.Swagger;

public static class SwaggerExtensions
{
    /// <summary>
    /// Safely adds or updates a response description in the OpenAPI operation
    /// </summary>
    public static void EnsureResponse(this OpenApiOperation operation, string statusCode, string description)
    {
        if (!operation.Responses.ContainsKey(statusCode))
        {
            operation.Responses[statusCode] = new OpenApiResponse { Description = description };
        }
        else
        {
            operation.Responses[statusCode].Description = description;
        }
    }
}
