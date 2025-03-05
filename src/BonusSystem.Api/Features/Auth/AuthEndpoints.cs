using BonusSystem.Shared.Dtos;

namespace BonusSystem.Api.Features.Auth;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/register", AuthHandlers.Register)
           .AllowAnonymous()
           .WithName("Register")
           .WithOpenApi();

        app.MapPost("/auth/login", AuthHandlers.Login)
           .AllowAnonymous()
           .WithName("Login")
           .WithOpenApi();

        return app;
    }
}
