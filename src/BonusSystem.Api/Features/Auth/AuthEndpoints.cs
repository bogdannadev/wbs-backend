using BonusSystem.Api.Infrastructure.Swagger;
using BonusSystem.Shared.Dtos;
using Microsoft.OpenApi.Models;

namespace BonusSystem.Api.Features.Auth;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/buyer_register", AuthHandlers.Register)
            .AllowAnonymous()
            .WithName("BuyerRegister")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Register a new user";
                operation.Description =
                    "Creates a new user account with the provided credentials and returns a JWT token.\n\n" +
                    "Request requires:\n" +
                    "- username: User display name (must be unique)\n" +
                    "- email: Email address used for login (must be unique)\n" +
                    "- password: Password (min 8 characters)\n" +
                    "Successful response contains:\n" +
                    "- userId: Unique identifier for the new user\n" +
                    "- token: JWT authentication token\n" +
                    "- role: User's role in the system";

                operation.EnsureResponse("200", "Registration successful");
                operation.EnsureResponse("400", "Registration failed");

                return operation;
            });

        group.MapPost("/register", AuthHandlers.Register)
            .RequireAuthorization()
            .WithName("Register")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Register a new user";
                operation.Description =
                    "Creates a new user account with the provided credentials and returns a JWT token.\n\n" +
                    "Request requires:\n" +
                    "- username: User display name (must be unique)\n" +
                    "- email: Email address used for login (must be unique)\n" +
                    "- password: Password (min 8 characters)\n" +
                    "- role: User role (0=Buyer, 1=Seller, 2=StoreAdmin, 3=SystemAdmin, 4=CompanyObserver, 5=SystemObserver)\n\n" +
                    "Successful response contains:\n" +
                    "- userId: Unique identifier for the new user\n" +
                    "- token: JWT authentication token\n" +
                    "- role: User's role in the system";

                operation.EnsureResponse("200", "Registration successful");
                operation.EnsureResponse("400", "Registration failed");

                return operation;
            });

        group.MapPost("/login", AuthHandlers.Login)
            .AllowAnonymous()
            .WithName("Login")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Authenticate user";
                operation.Description = "Authenticates a user with email and password and returns a JWT token.\n\n" +
                                        "Request requires:\n" +
                                        "- email: Email address used during registration\n" +
                                        "- password: User password\n\n" +
                                        "Successful response contains:\n" +
                                        "- userId: Unique identifier for the authenticated user\n" +
                                        "- token: JWT token to use in Authorization header\n" +
                                        "- role: User's role in the system";

                operation.EnsureResponse("200", "Authentication successful");
                operation.EnsureResponse("400", "Authentication failed");

                return operation;
            });

        return app;
    }
}